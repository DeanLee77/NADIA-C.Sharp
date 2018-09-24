using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactHashValue<T> : FactValue<T>
    {
        private String value;
        private String defaultValue;

        public FactHashValue(String hash)
        {
            SetValue(hash);
        }



        public void SetValue(String hash)
        {
            this.value = hash;
        }


        public override FactValueType GetFactValueType()
        {
            return FactValueType.HASH;
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
