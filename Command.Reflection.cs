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
        private void initializeParameters()
        {
            var fields = getParameterFields();
            Type[] typeArgs = new Type[] { typeof(string), typeof(string), typeof(Message) };

            foreach (var f in fields)
            {
                var ctr = getConstructor(f.FieldType, typeArgs);

                var nameAtt = f.GetCustomAttribute<Name>();
                var descAtt = f.GetCustomAttribute<Description>();
                var reqAtt = f.GetCustomAttribute<Required>();

                string name = nameAtt != null ? nameAtt.names[0] : "--" + f.Name;
                string description = descAtt != null ? descAtt.description : string.Empty;
                Message required = reqAtt != null ? reqAtt.message ?? Required.defaultMessage(name) : Message.NoError;

                var obj = ctr.Invoke(new object[] { name, description, required });
                Parameter par = obj as Parameter;

                parsers.Add(par);
                if (nameAtt == null)
                {
                    if (!RegexLookup.ArgumentName.IsMatch(name))
                        throw new ArgumentException("Argument name is illformed.", "alternatives");
                    parameters.Add(name, par);
                }
                else
                    foreach (var n in nameAtt.names)
                    {
                        if (!RegexLookup.ArgumentName.IsMatch(n))
                            throw new ArgumentException("Argument name is illformed.", "alternatives");
                        parameters.Add(n, par);
                    }

                f.SetValue(this, obj);
            }
        }

        private ConstructorInfo getConstructor(Type fieldType, Type[] typeArgs)
        {
            if (fieldType.IsGenericType && fieldType.GetGenericArguments()[0].IsArray)
            {
                var arrayType = fieldType.GetGenericArguments()[0].GetElementType();
                fieldType = typeof(ArrayParameter<>).MakeGenericType(arrayType);
            }
            return fieldType.GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, typeArgs, null);
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
