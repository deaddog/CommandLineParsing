namespace CommandLineParsing
{
    /// <summary>
    /// Represents a method that parses an array of strings into a value of type <typeparamref name="T"/>.
    /// This type of method is used to parse <see cref="Parameter"/> input.
    /// </summary>
    /// <typeparam name="T">The type to which the method parses <paramref name="args"/>.</typeparam>
    /// <param name="args">The strings that should be parsed.</param>
    /// <param name="result">The <typeparamref name="T"/> representation of <paramref name="args"/>, if parsing was successful; otherwise undefined.</param>
    /// <returns><see cref="Message.NoError"/>, if parsing was successful; otherwise a <see cref="Message"/> object describing the error in parsing.</returns>
    public delegate Message ParameterTryParse<T>(string[] args, out T result);
}
