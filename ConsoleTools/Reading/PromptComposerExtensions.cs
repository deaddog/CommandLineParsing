namespace ConsoleTools.Reading
{
    public static class PromptComposerExtensions
    {
        public static TComposer WithoutPrompt<TComposer>(this IPromptComposer<TComposer> composer) => composer.WithPrompt(ConsoleString.Empty);
    }
}
