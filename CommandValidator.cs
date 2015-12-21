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
