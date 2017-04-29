namespace CommandLineParsing
{
    /// <summary>
    /// Describes the different types of parameter requirements for parameters with the <see cref="Required"/> attribute.
    /// </summary>
    public enum RequirementType
    {
        /// <summary>
        /// If a value is not provided the parameter errors.
        /// </summary>
        Error,
        /// <summary>
        /// If a value is not provided the user is prompted for a value.
        /// </summary>
        Prompt,
        /// <summary>
        /// If a value is not provided the user is prompted for a value, but only if and when the <see cref="Parameter{T}.Value"/> property is used.
        /// </summary>
        PromptWhenUsed
    }
}
