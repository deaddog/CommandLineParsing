namespace ConsoleTools.Reading
{
    public interface IPromptComposer<TComposer>
    {
        ConsoleString Prompt { get; }
        TComposer WithPrompt(ConsoleString prompt);
    }
}
