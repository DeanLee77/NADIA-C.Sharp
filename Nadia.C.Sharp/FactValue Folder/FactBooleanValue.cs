using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactBooleanValue<T> : FactValue<T> 
    {
        private bool value;
        private bool? defaultValue; //bool? means nullable bool primative type

        public FactBooleanValue(bool booleanValue)
        {
            SetValue(booleanValue);
        }


        public void SetValue(bool booleanValue)
        {
            this.value = booleanValue;
        }

        public FactValue<T> NegatingValue()
        {
            return FactValue<T>.Parse(!this.value);
        }

        public override FactValueType GetFactValueType()
        {
            return FactValueType.BOOLEAN;
        }


        public override void SetDefaultValue(T defaultValue)
        {
            this.defaultValue = (bool)Convert.ChangeType(defaultValue, typeof(bool));
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
