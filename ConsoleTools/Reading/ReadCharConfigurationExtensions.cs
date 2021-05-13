namespace ConsoleTools.Reading
{
    public static class ReadCharConfigurationExtensions
    {
        public static ReadCharConfiguration<T> WithOption<T>(this ReadCharConfiguration<T> composer, char character, T value)
        {
            return new ReadCharConfiguration<T>
            (
                prompt: composer.Prompt,
                options: composer.Options.SetItem(character, value)
            );
        }
    }
}
