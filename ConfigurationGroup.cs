using System;
using System.Collections.Generic;

namespace CommandLineParsing
{
    /// <summary>
    /// Provides acces to multiple <see cref="IConfiguration"/> configurations through one element.
    /// </summary>
    public class ConfigurationGroup : IConfiguration
    {
        private LinkedList<IConfiguration> configurations;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGroup"/> class.
        /// </summary>
        public ConfigurationGroup()
        {
            this.configurations = new LinkedList<IConfiguration>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGroup"/> class.
        /// </summary>
        /// <param name="configurations">The configurations that are copied to the new <see cref="ConfigurationGroup"/>.
        /// Values in the first element will override any in the latter ones when they have equal keys.</param>
        public ConfigurationGroup(params IConfiguration[] configurations)
        {
            if (configurations == null)
                throw new ArgumentNullException(nameof(configurations));

            this.configurations = new LinkedList<IConfiguration>(configurations);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGroup"/> class.
        /// </summary>
        /// <param name="configurations">The configurations that are copied to the new <see cref="ConfigurationGroup"/>.
        /// Values in the first element will override any in the latter ones when they have equal keys.</param>
        public ConfigurationGroup(IEnumerable<IConfiguration> configurations)
        {
            if (configurations == null)
                throw new ArgumentNullException(nameof(configurations));

            this.configurations = new LinkedList<IConfiguration>(configurations);
        }
    }
}
