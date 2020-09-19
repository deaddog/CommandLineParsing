using System;

namespace CommandLineParsing
{
    /// <summary>
    /// Extends the <see cref="Validator"/> class with a set of validation methods for handling combinations of parameters.
    /// </summary>
    public class CommandValidator : Validator
    {
        private CommandEnsureCollection ensure;
        private CommandFailureCollection fail;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandValidator"/> class.
        /// </summary>
        public CommandValidator()
            : base()
        {
            this.ensure = new CommandEnsureCollection(this);
            this.fail = new CommandFailureCollection(this);
        }

        /// <summary>
        /// Gets an <see cref="CommandEnsureCollection"/> that provides methods to ensure that certain conditions are met.
        /// </summary>
        public new CommandEnsureCollection Ensure => ensure;
        /// <summary>
        /// Gets a <see cref="CommandFailureCollection"/> that provides methods to ensure that certain conditions are not met.
        /// </summary>
        public new CommandFailureCollection Fail => fail;

        /// <summary>
        /// Extends the <see cref="Validator.EnsureCollection"/> class with methods for ensuring parameter conditions are met.
        /// </summary>
        public class CommandEnsureCollection : EnsureCollection
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CommandEnsureCollection" /> class.
            /// </summary>
            /// <param name="validator">The <see cref="Validator"/> to which this <see cref="CommandEnsureCollection"/> should add validation methods.</param>
            public CommandEnsureCollection(Validator validator)
                : base(validator)
            {
            }

            /// <summary>
            /// Validates that at most one of <paramref name="parameters"/> is set.
            /// </summary>
            /// <param name="parameters">The collection of parameters to check.</param>
            /// <returns><see cref="Message.NoError"/> if zero or one <see cref="Parameter"/> is set; otherwise an error message describing the problem.</returns>
            public void ZeroOrOne(params Parameter[] parameters)
            {
                Add(() =>
                {
                    Parameter first = null;

                    for (int i = 0; i < parameters.Length; i++)
                        if (parameters[i].IsSet)
                        {
                            if (first == null)
                                first = parameters[i];
                            else
                                return new Message(string.Format("The {0} {1} cannot be used with the {2} {3}.",
                                    first.Name, first is FlagParameter ? "flag" : "parameter",
                                    parameters[i].Name, parameters[i] is FlagParameter ? "flag" : "parameter"));
                        }

                    return Message.NoError;
                });
            }

            /// <summary>
            /// Validates that if <paramref name="first"/> is set, none of <paramref name="parameters"/> are set.
            /// </summary>
            /// <param name="first">The first <see cref="Parameter"/>. If this is set, none of the remaining parameters can be set.</param>
            /// <param name="parameters">The <see cref="Parameter"/>s to check, if <paramref name="first"/> is set.</param>
            public void IfFirstNotRest(Parameter first, params Parameter[] parameters)
            {
                if (first == null)
                    throw new ArgumentNullException(nameof(first));

                Add(() =>
                {
                    if (!first.IsSet)
                        return Message.NoError;

                    for (int i = 0; i < parameters.Length; i++)
                        if (parameters[i].IsSet)
                            return new Message(string.Format("The {0} {1} cannot be used with the {2} {3}.",
                                first.Name, first is FlagParameter ? "flag" : "parameter",
                                parameters[i].Name, parameters[i] is FlagParameter ? "flag" : "parameter"));

                    return Message.NoError;
                });
            }
        }

        /// <summary>
        /// Extends the <see cref="Validator.FailureCollection"/> class with methods for failing validation based on parameters.
        /// </summary>
        public class CommandFailureCollection : FailureCollection
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CommandFailureCollection" /> class.
            /// </summary>
            /// <param name="validator">The <see cref="Validator"/> to which this <see cref="CommandFailureCollection"/> should add validation methods.</param>
            public CommandFailureCollection(Validator validator)
                : base(validator)
            {
            }
        }
    }
}
