using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineParsing
{
    public partial class Command
    {
        private static readonly string _COMMAND = typeof(Command).Name;
        private static readonly string _PARAMETER = typeof(Parameter).Name;
        private static readonly string _FLAGPARAMETER = typeof(FlagParameter).Name;
        private static readonly string _NAME = typeof(Name).Name;
        private static readonly string _NONAME = typeof(NoName).Name;
        private static readonly string _DESCRIPTION = typeof(Description).Name;
        private static readonly string _REQUIRED = typeof(Required).Name;
        private static readonly string _IGNORECASE = typeof(IgnoreCase).Name;
        private static readonly string _DEFAULT = typeof(Default).Name;

        private void initializeParameters()
        {
            var fields = getParameterFields();

            foreach (var f in fields)
            {
                var ctr = getConstructor(f.FieldType);

                var nameAtt = f.GetCustomAttribute<Name>();
                var descAtt = f.GetCustomAttribute<Description>();
                var reqAtt = f.GetCustomAttribute<Required>();
                var ignAtt = f.GetCustomAttribute<IgnoreCase>();
                var defAtt = f.GetCustomAttribute<Default>();

                if (ignAtt != null)
                {
                    if (f.FieldType == typeof(FlagParameter))
                        throw new TypeAccessException("A " + _FLAGPARAMETER + " cannot be marked with the " + _IGNORECASE + " attribute.");
                    if (f.FieldType.GetGenericTypeDefinition() == typeof(Parameter<>) &&
                        !(f.FieldType.GetGenericArguments()[0].IsEnum ||
                         (f.FieldType.GetGenericArguments()[0].IsArray && f.FieldType.GetGenericArguments()[0].GetElementType().IsEnum)))
                        throw new TypeAccessException("The " + _IGNORECASE + " attribute only applies to enumerations.");
                }

                if (defAtt != null)
                {
                    if (f.FieldType == typeof(FlagParameter))
                        throw new TypeAccessException("A " + _FLAGPARAMETER + " cannot have a default value.");
                }

                string name = nameAtt != null ? nameAtt.names[0] : "--" + f.Name;
                string description = descAtt != null ? descAtt.description : string.Empty;
                Message required = reqAtt != null ? reqAtt.message ?? Required.defaultMessage(name) : Message.NoError;
                bool ignore = ignAtt != null;
                object defaultValue = defAtt != null ? defAtt.Value : null;

                Parameter par;
                if (f.FieldType == typeof(FlagParameter))
                    par = ctr.Invoke(new object[] { name, description, required }) as Parameter;
                else if (f.FieldType.GetGenericTypeDefinition() == typeof(Parameter<>))
                    par = ctr.Invoke(new object[] { name, description, required, ignore }) as Parameter;
                else
                    throw new InvalidOperationException("Unknown parameter type: " + f.FieldType);

                if (defAtt != null)
                    f.FieldType.GetMethod("SetDefault").Invoke(par, new object[] { defaultValue });

                parsers.Add(par);
                if (nameAtt == null)
                {
                    if (!RegexLookup.ArgumentName.IsMatch(name))
                        throw new ArgumentException("Argument name \"" + name + "\" is illformed.", "name");
                    parameters.Add(name, par);
                }
                else
                    foreach (var n in nameAtt.names)
                    {
                        if (!RegexLookup.ArgumentName.IsMatch(n))
                            throw new ArgumentException("Argument name \"" + n + "\" is illformed.", "alternatives");
                        parameters.Add(n, par);
                    }

                f.SetValue(this, par);
            }
        }

        private ConstructorInfo getConstructor(Type fieldType)
        {
            if (fieldType == typeof(FlagParameter))
                return fieldType.GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(string), typeof(string), typeof(Message) },
                    null);

            if (fieldType.GetGenericTypeDefinition() == typeof(Parameter<>))
            {
                if (fieldType.GetGenericArguments()[0].IsArray)
                {
                    var arrayType = fieldType.GetGenericArguments()[0].GetElementType();
                    fieldType = typeof(ArrayParameter<>).MakeGenericType(arrayType);
                }
                return fieldType.GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(string), typeof(string), typeof(Message), typeof(bool) },
                    null);
            }

            throw new InvalidOperationException("Unknown parameter type: " + fieldType);
        }

        private FieldInfo[] getParameterFields()
        {
            var fields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var accepted = new List<FieldInfo>();

            for (int i = 0; i < fields.Length; i++)
            {
                if (!isFieldTypeParameter(fields[i].FieldType))
                    continue;

                if (!fields[i].IsInitOnly)
                    throw new FieldAccessException(string.Format("{0} fields must be defined as readonly - '{1}' in {2} is not.", typeof(Parameter).Name, fields[i].Name, fields[i].DeclaringType));

                if (fields[i].IsStatic)
                    throw new FieldAccessException(string.Format("The {0} field '{1}' in {2} is defined as static.", typeof(Parameter).Name, fields[i].Name, fields[i].DeclaringType));

                accepted.Add(fields[i]);
            }

            return accepted.ToArray();
        }
        private bool isFieldTypeParameter(Type fieldType)
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
