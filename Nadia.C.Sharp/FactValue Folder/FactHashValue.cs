using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactHashValue : FactValue
    {
        private string value;

        public FactHashValue(string hash)
        {
            SetValue(hash);
        }

        public void SetValue(string hash)
        {
            this.value = hash;
        }


        public override FactValueType GetFactValueType()
        {
            return FactValueType.HASH;
        }

        public string GetValue()
        {
            return this.value;
        }
    }
}
