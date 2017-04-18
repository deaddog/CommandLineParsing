using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace CommandLineParsing.Tests.RandomNames
{
    [TestFixture]
    public class NameServiceTests
    {
        [Test]
        public void LoadAndAssertNamesFM([Values(10, 500)] int count)
        {
            var nameservice = new NameService(true);

            var names = nameservice.GetNames(count, Genders.Both, false)?.ToList();

            Assert.IsNotNull(names);
            Assert.AreEqual(count, names.Count);

            names.Should()
                .NotBeNullOrEmpty().And
                .HaveCount(count);

            names.Should().NotContain(x => x.FirstName == null).And.NotContain(x => x.LastName == null);
        }
        [Test]
        public void LoadAndAssertNamesFMAlliteration([Values(10, 500)] int count)
        {
            var nameservice = new NameService(true);

            var names = nameservice.GetNames(count, Genders.Both, true)?.ToList();

            Assert.IsNotNull(names);
            Assert.AreEqual(count, names.Count);

            names.Should()
                .NotBeNullOrEmpty().And
                .HaveCount(count);

            names.Should().NotContain(x => x.FirstName == null).And.NotContain(x => x.LastName == null);
            names.Should().OnlyContain(x => x.FirstName[0] == x.LastName[0]);
        }

        [Test]
        public void LoadAndAssertNamesF([Values(10, 500)] int count)
        {
            var nameservice = new NameService(true);

            var names = nameservice.GetNames(count, Genders.Female , false)?.ToList();

            Assert.IsNotNull(names);
            Assert.AreEqual(count, names.Count);

            names.Should()
                .NotBeNullOrEmpty().And
                .HaveCount(count);

            names.Should().NotContain(x => x.FirstName == null).And.NotContain(x => x.LastName == null);
            names.Should().OnlyContain(x => x.Gender == Genders.Female);
        }
        [Test]
        public void LoadAndAssertNamesFAlliteration([Values(10, 500)] int count)
        {
            var nameservice = new NameService(true);

            var names = nameservice.GetNames(count, Genders.Female, true)?.ToList();

            Assert.IsNotNull(names);
            Assert.AreEqual(count, names.Count);

            names.Should()
                .NotBeNullOrEmpty().And
                .HaveCount(count);

            names.Should().NotContain(x => x.FirstName == null).And.NotContain(x => x.LastName == null);
            names.Should().OnlyContain(x => x.FirstName[0] == x.LastName[0]);
            names.Should().OnlyContain(x => x.Gender == Genders.Female);
        }

        [Test]
        public void LoadAndAssertNamesM([Values(10, 500)] int count)
        {
            var nameservice = new NameService(true);

            var names = nameservice.GetNames(count, Genders.Male, false)?.ToList();

            Assert.IsNotNull(names);
            Assert.AreEqual(count, names.Count);

            names.Should()
                .NotBeNullOrEmpty().And
                .HaveCount(count);

            names.Should().NotContain(x => x.FirstName == null).And.NotContain(x => x.LastName == null);
            names.Should().OnlyContain(x => x.Gender == Genders.Male);
        }
        [Test]
        public void LoadAndAssertNamesMAlliteration([Values(10, 500)] int count)
        {
            var nameservice = new NameService(true);

            var names = nameservice.GetNames(count, Genders.Male, true)?.ToList();

            Assert.IsNotNull(names);
            Assert.AreEqual(count, names.Count);

            names.Should()
                .NotBeNullOrEmpty().And
                .HaveCount(count);

            names.Should().NotContain(x => x.FirstName == null).And.NotContain(x => x.LastName == null);
            names.Should().OnlyContain(x => x.FirstName[0] == x.LastName[0]);
            names.Should().OnlyContain(x => x.Gender == Genders.Male);
        }
    }
}
