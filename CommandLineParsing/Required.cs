using CommandLineParsing.Output;
using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Specifies that a parameter is required; it must be used when executing the containing command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class Required : Attribute
    {
        internal readonly RequirementType requirementType;
        internal readonly Message message;

        /// <summary>
        /// Initializes a new instance of the <see cref="Required"/> class, using a simple predefined error message.
        /// </summary>
        /// <param name="requirementType">The level of requirement for the parameter.</param>
        public Required(RequirementType requirementType = RequirementType.Error)
        {
            this.message = null;
            this.requirementType = requirementType;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Required"/> class, using a custom error message.
        /// </summary>
        /// <param name="message">The message that should be displayed if the parameter is not used.
        /// <paramref name="requirementType"/> value.</param>
        /// <param name="requirementType">The level of requirement for the parameter.</param>
        public Required(string message, RequirementType requirementType = RequirementType.Error)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            this.message = new Message(ConsoleString.FromContent(message));
            this.requirementType = requirementType;
        }

        internal static Message defaultMessage(string name, RequirementType requirementType)
        {
            switch (requirementType)
            {
                case RequirementType.Error:
                    return new Message($"No value specified for the required parameter \"{name}\"");
                case RequirementType.Prompt:
                case RequirementType.PromptWhenUsed:
                    return new Message($"Specify a value for the \"{name}\" parameter: ");

                default:
                    throw new ArgumentException($"Invalid {nameof(RequirementType)} value.", nameof(requirementType));
            }
        }
    }
}
