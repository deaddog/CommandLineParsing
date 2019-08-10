using System;

#pragma warning disable CS1591 // Ignore comments in visitor
namespace CommandLineParsing.Output.Formatting.Structure
{
    public abstract class FormatVisitor<TResult>
    {
        public TResult Visit(FormatElement format)
        {
            switch (format)
            {
                case FormatColorElement color: return Visit(color);
                case FormatConcatenationElement concatenation: return Visit(concatenation);
                case FormatConditionElement condition: return Visit(condition);
                case FormatFunctionElement function: return Visit(function);
                case FormatNoContentElement noContent: return Visit(noContent);
                case FormatTextElement text: return Visit(text);
                case FormatVariableElement variable: return Visit(variable);

                default:
                    throw new NotSupportedException($"The element type {format.GetType().Name} is not supported.");
            }
        }

        public abstract TResult Visit(FormatColorElement format);
        public abstract TResult Visit(FormatConcatenationElement format);
        public abstract TResult Visit(FormatConditionElement format);
        public abstract TResult Visit(FormatFunctionElement format);
        public abstract TResult Visit(FormatNoContentElement format);
        public abstract TResult Visit(FormatTextElement format);
        public abstract TResult Visit(FormatVariableElement format);
    }

    public abstract class FormatVisitor<TResult, TArg>
    {
        public TResult Visit(FormatElement format, TArg arg)
        {
            switch (format)
            {
                case FormatColorElement color: return Visit(color, arg);
                case FormatConcatenationElement concatenation: return Visit(concatenation, arg);
                case FormatConditionElement condition: return Visit(condition, arg);
                case FormatFunctionElement function: return Visit(function, arg);
                case FormatNoContentElement noContent: return Visit(noContent, arg);
                case FormatTextElement text: return Visit(text, arg);
                case FormatVariableElement variable: return Visit(variable, arg);

                default:
                    throw new NotSupportedException($"The element type {format.GetType().Name} is not supported.");
            }
        }

        public abstract TResult Visit(FormatColorElement format, TArg arg);
        public abstract TResult Visit(FormatConcatenationElement format, TArg arg);
        public abstract TResult Visit(FormatConditionElement format, TArg arg);
        public abstract TResult Visit(FormatFunctionElement format, TArg arg);
        public abstract TResult Visit(FormatNoContentElement format, TArg arg);
        public abstract TResult Visit(FormatTextElement format, TArg arg);
        public abstract TResult Visit(FormatVariableElement format, TArg arg);
    }
#pragma warning restore CS1591 // Ignore comments in visitor
}
