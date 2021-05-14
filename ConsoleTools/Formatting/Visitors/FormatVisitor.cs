using ConsoleTools.Formatting.Structure;
using System;

namespace ConsoleTools.Formatting.Visitors
{
    public abstract class FormatVisitor<TResult>
    {
        public TResult Visit(Format format)
        {
            switch (format)
            {
                case ColorFormat color: return Visit(color);
                case ConcatenationFormat concatenation: return Visit(concatenation);
                case ConditionFormat condition: return Visit(condition);
                case FunctionFormat function: return Visit(function);
                case NoContentFormat noContent: return Visit(noContent);
                case TextFormat text: return Visit(text);
                case VariableFormat variable: return Visit(variable);

                default:
                    throw new NotSupportedException($"The element type {format.GetType().Name} is not supported.");
            }
        }

        public abstract TResult Visit(ColorFormat format);
        public abstract TResult Visit(ConcatenationFormat format);
        public abstract TResult Visit(ConditionFormat format);
        public abstract TResult Visit(FunctionFormat format);
        public abstract TResult Visit(NoContentFormat format);
        public abstract TResult Visit(TextFormat format);
        public abstract TResult Visit(VariableFormat format);
    }

    public abstract class FormatVisitor<TResult, TArg>
    {
        public TResult Visit(Format format, TArg arg)
        {
            switch (format)
            {
                case ColorFormat color: return Visit(color, arg);
                case ConcatenationFormat concatenation: return Visit(concatenation, arg);
                case ConditionFormat condition: return Visit(condition, arg);
                case FunctionFormat function: return Visit(function, arg);
                case NoContentFormat noContent: return Visit(noContent, arg);
                case TextFormat text: return Visit(text, arg);
                case VariableFormat variable: return Visit(variable, arg);

                default:
                    throw new NotSupportedException($"The element type {format.GetType().Name} is not supported.");
            }
        }

        public abstract TResult Visit(ColorFormat format, TArg arg);
        public abstract TResult Visit(ConcatenationFormat format, TArg arg);
        public abstract TResult Visit(ConditionFormat format, TArg arg);
        public abstract TResult Visit(FunctionFormat format, TArg arg);
        public abstract TResult Visit(NoContentFormat format, TArg arg);
        public abstract TResult Visit(TextFormat format, TArg arg);
        public abstract TResult Visit(VariableFormat format, TArg arg);
    }
}
