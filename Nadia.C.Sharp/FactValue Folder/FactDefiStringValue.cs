using System;
using System.Text.RegularExpressions;

namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactDefiStringValue<T> : FactValue<T>
    {
        private string value;
        private string defaultValue;
        private string pattern = @"^("")(.*)("")(.)*";
        //private Regex regex = new Regex(pattern);
        private Match match;

        public FactDefiStringValue(string s)
        {
            match = Regex.Match(s, pattern);
            if (match.Success)
            {
                SetValue(match.Groups[2].Value);
            }
        }
        public void SetValue(string s)
        {
            this.value = s;
        }


        public override FactValueType GetFactValueType()
        {
            return FactValueType.DEFI_STRING;
        }

        public override void SetDefaultValue(T defaultValue)
        {
            this.defaultValue = defaultValue.ToString();
        }

        public override T GetValue()
        {
            // TODO Auto-generated method stub
            return (T)Convert.ChangeType(this.value, typeof(T));
        }

        public override T GetDefaultValue()
        {
            // TODO Auto-generated method stub
            return (T)Convert.ChangeType(this.defaultValue, typeof(T));
        }
    }
}
