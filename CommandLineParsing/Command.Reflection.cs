using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CommandLineParsing
{
    public partial class Command
    {
        private void initializeParameters()
        {
            var fields = getParameterFields();

            foreach (var f in fields)
            {
                var nameAtt = f.GetCustomAttribute<Name>();
                var nonAtt = f.GetCustomAttribute<NoName>();
                var descAtt = f.GetCustomAttribute<Description>();
                var reqAtt = f.GetCustomAttribute<Required>();
                var ignAtt = f.GetCustomAttribute<IgnoreCase>();
                var defAtt = f.GetCustomAttribute<Default>();

                bool isFlag = f.FieldType == typeof(FlagParameter);
                bool isTyped = f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(Parameter<>);

                if (!isFlag && !isTyped)
                    throw new InvalidOperationException($"Unknown parameter type: {f.FieldType}");

                var paramType = isTyped ? f.FieldType.GetGenericArguments()[0] : null;
                bool isArray = !isFlag && paramType.IsArray;
                var elementType = isTyped ? (isArray ? paramType.GetElementType() : paramType) : null;
                bool isEnum = isTyped && elementType.IsEnum;

                if (ignAtt != null)
                {
                    if (isFlag)
                        throw new TypeAccessException($"A {nameof(FlagParameter)} cannot be marked with the {nameof(IgnoreCase)} attribute.");
                    if (!isEnum)
                        throw new TypeAccessException($"The {nameof(IgnoreCase)} attribute only applies to enum types.");
                }

                if (defAtt != null)
                {
                    if (isFlag)
                        throw new TypeAccessException($"A {nameof(FlagParameter)} cannot have a default value.");
                }

                if (reqAtt != null)
                {
                    if (isFlag)
                        throw new TypeAccessException($"A {nameof(FlagParameter)} cannot be have the {nameof(Required)} attribute.");
                }

                if (nonAtt != null)
                {
                    if (nameAtt != null)
                        throw new TypeAccessException($"A {nameof(Parameter)} cannot have the {nameof(Name)} and the {nameof(NoName)} attribute simultaneously.");

                    if (parameters.HasNoName)
                        throw new TypeAccessException($"A {nameof(Command)} can only support a single {nameof(Parameter)} with the {nameof(NoName)} attribute.");

                    if (!isArray)
                        throw new TypeAccessException($"A {nameof(Parameter)} with the {nameof(NoName)} attribute must be defined as generic, using an array as type argument.");

                    if (reqAtt != null)
                        throw new TypeAccessException($"A {nameof(Parameter)} with the {nameof(NoName)} attribute cannot have the {nameof(Required)} attribute.");
                }

                string name = nonAtt != null ? null : (nameAtt?.name ?? GetDefaultFieldName(f));
                string[] alternatives = nameAtt?.alternatives ?? new string[0];
                string description = descAtt?.description ?? string.Empty;
                RequirementType? requirementType = reqAtt?.requirementType;
                Message required = reqAtt == null ? Message.NoError : (reqAtt.message ?? Required.defaultMessage(name, reqAtt.requirementType));
                bool ignore = ignAtt != null;
                object defaultValue = defAtt?.value;

                Parameter par;
                if (isFlag)
                    par = new FlagParameter(name, alternatives, description);
                else if (f.FieldType.GetGenericTypeDefinition() == typeof(Parameter<>))
                    par = constructParameter(paramType, name, alternatives, description, requirementType, required, ignore);
                else
                    throw new InvalidOperationException($"Unknown parameter type: {f.FieldType}");

                if (defAtt != null)
                    f.FieldType.GetMethod("SetDefault").Invoke(par, new object[] { defaultValue });

                parameters.Add(par);
                f.SetValue(this, par);
            }
        }

        private Parameter constructParameter(Type type, string name, string[] alternatives, string description, RequirementType? requirementType, Message required, bool enumIgnore)
        {
            Type paramType = typeof(Parameter<>).MakeGenericType(type);

            var ctr = paramType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];

            return (Parameter)ctr.Invoke(new object[] { name, alternatives, description, requirementType, required, enumIgnore });
        }

        private string GetDefaultFieldName(FieldInfo field)
        {
            var name = field.Name;

            name = name.Trim('_');
            name = name.Replace('_', '-');
            var sb = new StringBuilder(name);
            for (int i = 0; i < sb.Length; i++)
                if (char.IsUpper(sb[i]))
                {
                    sb[i] = char.ToLower(sb[i]);
                    sb.Insert(i, '-');
                }

            return $"--{sb}";
        }

        private FieldInfo[] getParameterFields()
        {
            return getParameterFields(GetType());
        }
        private static FieldInfo[] getParameterFields(Type commandType)
        {
            var fields = commandType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var accepted = new List<FieldInfo>();

            for (int i = 0; i < fields.Length; i++)
            {
                if (!isFieldTypeParameter(fields[i].FieldType))
                    continue;

                if (!fields[i].IsInitOnly)
                    throw new FieldAccessException($"{nameof(Parameter)} fields must be defined as readonly - '{fields[i].Name}' in {fields[i].DeclaringType} is not.");

                if (fields[i].IsStatic)
                    throw new FieldAccessException($"The {nameof(Parameter)} field '{fields[i].Name}' in {fields[i].DeclaringType} is defined as static.");

                accepted.Add(fields[i]);
            }

            if (commandType.BaseType != typeof(Command))
                accepted.AddRange(getParameterFields(commandType.BaseType));

            return accepted.ToArray();
        }
        private static bool isFieldTypeParameter(Type fieldType)
        {
            if (fieldType == typeof(FlagParameter))
                return true;
            else if (fieldType.IsGenericType)
                return fieldType.GetGenericTypeDefinition() == typeof(Parameter<>);
            else
                return false;
        }
    }
}
