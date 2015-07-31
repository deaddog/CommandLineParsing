using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParsing
{
    /// <summary>
    /// Represents a message displaying a list of alternatives when an unknown argument is found.
    /// Alternatives are automatically sorted according to their edit distance.
    /// </summary>
    public class UnknownArgumentMessage : Message
    {
        /// <summary>
        /// An enumeration of the different types of interpretations of arguments.
        /// </summary>
        public enum ArgumentType
        {
            /// <summary>
            /// Indicates that the unknown argument should be a sub command.
            /// </summary>
            SubCommand,
            /// <summary>
            /// Indicates that the unknown argument should be a parameter.
            /// </summary>
            Parameter
        }
        private static string argumentTypeString(ArgumentType arg)
        {
            switch (arg)
            {
                case ArgumentType.SubCommand: return "subcommand";
                case ArgumentType.Parameter: return "parameter";
                default:
                    throw new ArgumentOutOfRangeException("arg");
            }
        }

        private static Func<string, string, uint> editDistance = EditDistance.GetEditDistanceMethod(1, 4, 1, 1);

        private string argument;
        private ArgumentType argumentType;
        private Dictionary<string, string> alternativeArguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownArgumentMessage"/> class.
        /// </summary>
        /// <param name="argument">The argument that could not be handled.</param>
        /// <param name="argumentType">The type that was expected from the argument. This will affect the displayed message.</param>
        public UnknownArgumentMessage(string argument, ArgumentType argumentType)
        {
            this.argument = argument;
            this.argumentType = argumentType;
            this.alternativeArguments = new Dictionary<string, string>();
        }

        /// <summary>
        /// Adds an alternative to the <see cref="UnknownArgumentMessage"/> along with a description.
        /// </summary>
        /// <param name="alternative">The alternative.</param>
        /// <param name="description">The associated description.</param>
        public void AddAlternative(string alternative, string description)
        {
            if (!this.alternativeArguments.ContainsKey(alternative))
                this.alternativeArguments.Add(alternative, description);
        }

        /// <summary>
        /// Adds a set of alternatives to the <see cref="UnknownArgumentMessage"/> along with a description.
        /// Only a single string from <paramref name="alternatives"/> is listed (the one with the lowest edit-distance).
        /// </summary>
        /// <param name="alternatives">The set of alternatives.</param>
        /// <param name="description">The associated description.</param>
        public void AddAlternative(string[] alternatives, string description)
        {
            AddAlternative(EditDistance.OrderByDistance(alternatives, argument, editDistance).First().Item1, description);
        }

        /// <summary>
        /// Gets a string representing this <see cref="UnknownArgumentMessage" />.
        /// </summary>
        /// <returns>A message containing an initial message and a list of alternatives for the unknown command.</returns>
        public override string GetMessage()
        {
            if (alternativeArguments.Count == 0)
                switch (argumentType)
                {
                    case ArgumentType.SubCommand: return string.Format("The executed command does not support any sub-commands. [Yellow:{0}] is invalid.", argument);
                    case ArgumentType.Parameter: return string.Format("The executed command does not support any parameters. [Yellow:{0}] is invalid.", argument);
                }

            string message = string.Format("{0} [Yellow:{1}] was not recognized. Did you mean any of the following:", argumentTypeString(argumentType), argument);
            var list = EditDistance.OrderByDistance(alternativeArguments.Keys, argument, editDistance).TakeWhile((arg, i) => i == 0 || arg.Item2 < 5).Select(x => x.Item1).ToArray();
            var strLen = list.Max(x => x.Length);

            foreach (var a in list)
                message += "\n  " + a.PadRight(strLen + 2) + alternativeArguments[a];
            return message;
        }
    }
}
