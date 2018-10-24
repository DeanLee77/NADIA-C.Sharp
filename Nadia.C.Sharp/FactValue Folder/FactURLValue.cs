using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactURLValue : FactValue
    {

        private string value;

        public FactURLValue(string url)
        {
            SetValue(url);
        }


        public void SetValue(string url)
        {
            this.value = url;
        }

        public override FactValueType GetFactValueType()
        {
            return FactValueType.URL;
        }

        public string GetValue()
        {
            return this.value;
        }
    }
}
