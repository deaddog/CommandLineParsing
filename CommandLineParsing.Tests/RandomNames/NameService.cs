using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandLineParsing.Tests.RandomNames
{
    public class NameService
    {
        private const int CACHE_SIZE = 500;

        private readonly bool _useCache;
        private readonly Lazy<RestClient> _client;

        public NameService(bool useCache)
        {
            _useCache = useCache;
            _client = new Lazy<RestClient>(() => new RestClient("http://listofrandomnames.com/index.cfm?generated="));
        }

        private static string GetGenderArgument(Genders genders)
        {
            switch (genders)
            {
                case Genders.Male: return "m";
                case Genders.Female: return "f";
                case Genders.Both: return "na";

                default:
                    throw new ArgumentOutOfRangeException(nameof(genders), $"Unknown gender \"{genders}\"");
            }
        }
        private static string GetAliterationArgument(bool useAlliteration)
        {
            return useAlliteration ? "1" : "0";
        }

        public IEnumerable<Name> GetNames(int count, Genders genders = Genders.Both, bool useAlliteration = false)
        {
            if (!_useCache)
                return GetNamesOnline(count, genders, useAlliteration);

            if (count > CACHE_SIZE)
                return GetNames(CACHE_SIZE, genders, useAlliteration).Concat(GetNamesOnline(count - CACHE_SIZE, genders, useAlliteration));
            else
            {
                var cache = GetNamesFromCache(genders, useAlliteration)?.ToList();

                if (cache == null)
                {
                    cache = GetNamesOnline(CACHE_SIZE, genders, useAlliteration).Take(CACHE_SIZE).ToList();
                    SaveToCache(cache, genders, useAlliteration);
                }

                Random rnd = new Random();

                List<Name> names = new List<Name>(count);
                while (names.Count < count)
                {
                    var index = rnd.Next(cache.Count);
                    names.Add(cache[index]);
                    cache.RemoveAt(index);
                }

                return names;
            }
        }

        public IEnumerable<Name> GetNamesFromCache(Genders genders, bool useAlliteration)
        {
            var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Combine(dir, "names_" + GetGenderArgument(genders) + "_" + GetAliterationArgument(useAlliteration) + ".json");

            if (File.Exists(filePath))
                return JsonConvert.DeserializeObject<List<Name>>(File.ReadAllText(filePath));
            else
                return null;
        }
        private void SaveToCache(IEnumerable<Name> names, Genders genders, bool useAlliteration)
        {
            var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Combine(dir, "names_" + GetGenderArgument(genders) + "_" + GetAliterationArgument(useAlliteration) + ".json");

            File.WriteAllText(filePath, JsonConvert.SerializeObject(names.ToList()));
        }

        public IEnumerable<Name> GetNamesOnline(int count, Genders genders, bool useAlliteration)
        {
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"action=main.generate&numberof={count}&nameType={GetGenderArgument(genders)}&fnameonly=0&allit={GetAliterationArgument(useAlliteration)}", ParameterType.RequestBody);
            var response = _client.Value.Execute(request);

            var doc = new HtmlDocument();
            doc.LoadHtml(response.Content);

            var list = doc.DocumentNode.SelectNodes("//ul[@id='nameres']/li");

            foreach (var i in list)
            {
                var genderStr = i.GetAttributeValue("class", null)?.Trim();
                var anchors = i.SelectNodes("a");
                var firstName = anchors[0].InnerText.Trim();
                var lastName = anchors[1].InnerText.Trim();

                var gender = genderStr == "list_F" ? Genders.Female : Genders.Male;

                yield return new Name(firstName, lastName, gender);
            }
        }
    }
}
