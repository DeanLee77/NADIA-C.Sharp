using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactStringValue<T> : FactValue<T>
    {
        private string value;
        private string defaultValue;

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
