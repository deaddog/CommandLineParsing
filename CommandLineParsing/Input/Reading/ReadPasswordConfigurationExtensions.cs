using CommandLineParsing.Output;

namespace CommandLineParsing.Input.Reading
{
    public static class ReadPasswordConfigurationExtensions
    {
        public static ReadPasswordConfiguration WithPrompt(this ReadPasswordConfiguration composer, ConsoleString prompt)
        {
            return new ReadPasswordConfiguration
            (
                prompt: prompt,
                renderAs: composer.RenderAs,
                repeatRender: composer.RepeatRender
            );
        }
        public static ReadPasswordConfiguration WithoutPrompt(this ReadPasswordConfiguration composer) => composer.WithPrompt(ConsoleString.Empty);

        public static ReadPasswordConfiguration WithRender(this ReadPasswordConfiguration composer, char render, bool repeat)
        {
            return new ReadPasswordConfiguration
            (
                prompt: composer.Prompt,
                renderAs: ConsoleString.FromContent(render.ToString()),
                repeatRender: repeat
            );
        }
        public static ReadPasswordConfiguration WithRender(this ReadPasswordConfiguration composer, ConsoleString render, bool repeat)
        {
            return new ReadPasswordConfiguration
            (
                prompt: composer.Prompt,
                renderAs: render,
                repeatRender: repeat
            );
        }
        public static ReadPasswordConfiguration WithoutRender(this ReadPasswordConfiguration composer) => composer.WithRender(ConsoleString.Empty, false);
    }
}
