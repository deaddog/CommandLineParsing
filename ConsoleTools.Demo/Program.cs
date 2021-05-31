using ConsoleTools.Applications;
using ConsoleTools.Coloring;
using ConsoleTools.Formatting;
using ConsoleTools.Parsing;
using ConsoleTools.Reading;
using System;
using System.Collections.Immutable;

namespace ConsoleTools.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Command.Create()
                .RunAsync(Consoles.System, args)
                .Wait();
            return;


            var console = Consoles.SystemCustom(ColorTable.Default
                .With(Colors.ErrorValue, ConsoleColor.White, ConsoleColor.DarkRed)
                .With("warning", ConsoleColor.DarkYellow)
            );

            var fm = Formatter<Person>.Empty
                .With("name", p => p.Name, v => v
                    .WithPaddedLength(4)
                )
                .WithTyped("age", p => p.Age, v => v.WithAutoColor(a => a switch
                {
                    > 50 and < 60 => "red",
                    < 30 => "green",
                    _ => null
                }))
                .WithListFunction("aliases", p => p.Aliases, aliasfm => aliasfm
                    .With("name", x => x, n => n.WithAutoColor(v => v.ToLower() switch
                    {
                        "sir" => Colors.ErrorMessage,
                        "man" => Colors.ErrorValue,
                        "human" => Colors.UnknownFormatCondition,
                        _ => Color.NoColor
                    }))
                );
            var s = fm.Format(Formatting.Structure.Format.Parse("$name (@aliases{[auto:$name],[warning:\\,] , and }) is [auto:$age] years old"), new Person("Peter", 53, "sir", "human", "man", "person", "guy"));

            console.WriteLine(s);

            console.ReadKey(true); return;
            //var n = console.ReadLine<int>(c => c.WithPrompt("tal: ").Where(x => x > 0));
            //console.WriteLine($"My number: [red:{n}]");
            //return;

            try
            {
                var number = console.ReadLine<Person>(c => c
                    .WithPrompt("Value please: ")
                    .WithParser(new Parsing.Parser<Person>(x => new Message<Person>(new Person(x))))
                    .WithRegexParser("^(?<name>\\w+) *(?<age>\\d+)$", m => new Person(m.Groups["name"].Value))
                //.Where(v => v > 25, "Such low values are [red:not] supported")
                );

                console.WriteLine($"The value was: [green:{number}]");
            }
            catch (MessageException msg)
            {
                console.WriteLine(msg.Message);
            }

            if (console.ReadCharOrCancel<char>(c => c
                 .WithPrompt("Select your preference:\na) bleh\nb) blah")
                 .WithOption('a', 'a')
                 .WithOption('b', 'b'),
                out var r
            ))
            {
                console.WriteLine($"You selected [blue:{r}]");
            }
            else
                console.WriteLine("You cancelled");

            var v = console.ReadChar<bool>(c => c
                .WithOption('y', true)
                .WithOption('n', false)
                .WithPrompt("Is this okay? (y/n) ")
            );

            console.WriteLine($"You selected [green:{v}]");
        }
    }

    public class Person
    {
        public Person(string name, int age = 25, params string[] aliases)
        {
            Name = name;
            Age = age;
            Aliases = aliases;
        }

        public int Age { get; }
        public string Name { get; }
        public string[] Aliases { get; }
    }
}
