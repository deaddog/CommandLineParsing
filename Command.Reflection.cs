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
