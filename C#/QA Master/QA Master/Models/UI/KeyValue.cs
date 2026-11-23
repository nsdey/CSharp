using System.ComponentModel;
using System.Reflection;

namespace QA_Master.Models.UI
{
    public class KeyValue(string? key, string? value)
    {
        public string? Key => key;
        public string? Value => value;

        public override string? ToString()
        {
            if (Value != null)
                return Value;
            else
                return base.ToString();
        }

        public static List<KeyValue>? GetKeyValues(Enum @enum)
        {
            string[] enumNames = Enum.GetNames(@enum.GetType());

            List<KeyValue> result = new List<KeyValue>();

            for (int i = 0; i < enumNames.Length; i++)
            {
                string enumDescription = enumNames[i];

                MemberInfo[] memberInfos = @enum.GetType().GetMember(enumNames[i]);

                if (memberInfos != null && memberInfos.Length > 0)
                {
                    object[] attrs = memberInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        enumDescription = ((DescriptionAttribute)attrs[0]).Description;
                    }
                }

                result.Add(new KeyValue(enumNames[i], enumDescription));
            }

            return result;
        }
    }
}
