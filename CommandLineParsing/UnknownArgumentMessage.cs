using CommandLineParsing.Output;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace CommandLineParsing
{
    public static class UnknownArgumentMessage
    {
        private enum ArgumentType
        {
            SubCommand,
            Parameter
        }

        private static Func<string, string, uint> _editDistance = EditDistance.GetEditDistanceMethod(1, 4, 100, 1);

        public static Message FromSubcommands(Command command, string argument)
        {
            return new Message(GetMessageContent
            (
                argument: argument,
                argumentType: ArgumentType.SubCommand,
                alternativeArguments: command.SubCommands.ToImmutableDictionary
                (
                    keySelector: c => c.Key,
                    elementSelector: c => c.Value.Description
                )
            ));
        }
        public static Message FromParameters(Command command, string argument)
        {
            return new Message(GetMessageContent
            (
                argument: argument,
                argumentType: ArgumentType.Parameter,
                alternativeArguments: command.Parameters.ToImmutableDictionary
                (
                    keySelector: p => GetParameterBestMatch(p, argument),
                    elementSelector: p => p.Description
                )
            ));
        }

        private static string GetParameterBestMatch(Parameter parameter, string argument)
        {
            return EditDistance.OrderByDistance
            (
                collection: parameter.GetNames(true),
                origin: argument,
                editdistance: _editDistance
            ).First().Item1;
        }
        private static string ArgumentTypeString(ArgumentType arg)
        {
            return arg switch
            {
                ArgumentType.SubCommand => "subcommand",
                ArgumentType.Parameter => "parameter",

                _ => throw new ArgumentOutOfRangeException("arg"),
            };
        }

        private static ConsoleString GetMessageContent(string argument, ArgumentType argumentType, IImmutableDictionary<string, string> alternativeArguments)
        {
            if (alternativeArguments.Count == 0)
                switch (argumentType)
                {
                    case ArgumentType.SubCommand: return "The executed command does not support any sub-commands. " + ConsoleString.FromContent(argument, Color.NoColor.WithForeground("Yellow")) + " is invalid.";
                    case ArgumentType.Parameter: return $"The executed command does not support any parameters. " + ConsoleString.FromContent(argument, Color.NoColor.WithForeground("Yellow")) + " is invalid.";
                }

            string message = $"{ArgumentTypeString(argumentType)} [Yellow:{argument}] was not recognized. Did you mean any of the following:";
            var list = EditDistance.OrderByDistance(alternativeArguments.Keys, argument, _editDistance).TakeWhile((arg, i) => i == 0 || arg.Item2 < 5).Select(x => x.Item1).ToArray();
            var strLen = list.Max(x => x.Length);

            foreach (var a in list)
                message += "\n  " + a.PadRight(strLen + 2) + alternativeArguments[a];
            return message;
        }
    }
}
