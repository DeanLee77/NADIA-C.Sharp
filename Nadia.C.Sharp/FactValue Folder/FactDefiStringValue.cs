using System;
using System.Text.RegularExpressions;

namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactDefiStringValue : FactValue
    {
        private string value;

        private string pattern = @"^([""\“])(.*)([""\”])(\.)*";
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

        public string GetValue()
        {
            // TODO Auto-generated method stub
            return this.value;
        }
    }
}
