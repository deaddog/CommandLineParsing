using CommandLineParsing.Output;
using System;

namespace CommandLineParsing.Input.Reading
{
    public class ReadPasswordConfiguration
    {
        public static ReadPasswordConfiguration Create()
        {
            return new ReadPasswordConfiguration
            (
                prompt: ConsoleString.Empty,
                renderAs: "*",
                repeatRender: true
            );
        }

        public ReadPasswordConfiguration(ConsoleString prompt, ConsoleString renderAs, bool repeatRender)
        {
            Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));

            RenderAs = renderAs ?? throw new ArgumentNullException(nameof(renderAs));
            RepeatRender = repeatRender;
        }

        public ConsoleString Prompt { get; }

        public ConsoleString RenderAs { get; }
        public bool RepeatRender { get; }
    }
}
