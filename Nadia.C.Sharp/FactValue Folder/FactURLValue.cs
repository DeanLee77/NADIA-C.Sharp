using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactURLValue<T> : FactValue<T>
    {

        private String value;
        private String defaultValue;

        public FactURLValue(String url)
        {
            SetValue(url);
        }



        public void SetValue(String url)
        {
            this.value = url;
        }

        public override FactValueType GetFactValueType()
        {
            return FactValueType.URL;
        }



        public override void SetDefaultValue(T defaultValue)
        {
             this.defaultValue = (string)Convert.ChangeType(defaultValue, typeof(string));
        }



        public override T GetValue()
        {
            return (T)Convert.ChangeType(this.value, typeof(T));
        }


        public override T GetDefaultValue()
        {
            return (T)Convert.ChangeType(this.defaultValue, typeof(T));
        }

    }
}
