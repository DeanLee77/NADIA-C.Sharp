using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactStringValue : FactValue
    {
        private string value;

        public FactStringValue(string s)
        {
            SetValue(s);
        }


        public void SetValue(string s)
        {
            this.value = s;
        }


        public override FactValueType GetFactValueType()
        {
            return FactValueType.STRING;
        }

        public string GetValue()
        {
            // TODO Auto-generated method stub
            return this.value;
        }
    }
}
