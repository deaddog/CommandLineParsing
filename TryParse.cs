namespace CommandLineParsing
{
    public delegate bool TryParse<T>(string s, out T result);
}
