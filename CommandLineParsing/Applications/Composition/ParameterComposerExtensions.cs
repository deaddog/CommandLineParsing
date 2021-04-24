using System;

namespace CommandLineParsing.Applications.Composition
{
    public static class ParameterComposerExtensions
    {
        public static ParameterComposer<T> WithEnvVar<T>(this ParameterComposer<T> parameter, string environmentVariable)
        {
            return new ParameterComposer<T>
            (
                names: parameter.Names,
                description: parameter.Description,
                parser: parameter.Parser,
                validator: parameter.Validator,
                environmentVariable: environmentVariable,
                usage: parameter.Usage
            );
        }

        public static ParameterComposer<T> Required<T>(this ParameterComposer<T> parameter, bool required = true)
        {
            return new ParameterComposer<T>
            (
                names: parameter.Names,
                description: parameter.Description,
                parser: parameter.Parser,
                validator: parameter.Validator,
                environmentVariable: parameter.EnvironmentVariable,
                usage: parameter.Usage switch
                {
                    Usage.ZeroOrOne => required ? Usage.One : Usage.ZeroOrOne,
                    Usage.One => required ? Usage.One : Usage.ZeroOrOne,
                    Usage.ZeroOrMany => required ? Usage.OneOrMany : Usage.ZeroOrMany,
                    Usage.OneOrMany => required ? Usage.OneOrMany : Usage.ZeroOrMany,

                    _ => throw new NotSupportedException()
                }
            );
        }
    }
}
