using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactDoubleValue<T> : FactValue<T>
    {
        private double value;
        private double? defaultValue; //double? means nullable double primative type

        public FactDoubleValue(double d)
        {
            SetValue(d);
        }

        public void SetValue(double d)
        {
            this.value = d;
        }


        public override T GetValue()
        {
            return (T)Convert.ChangeType(this.value, typeof(T));
        }



        public override T GetDefaultValue()
        {
            return (T)Convert.ChangeType(this.defaultValue, typeof(T));
        }


        public override FactValueType GetFactValueType()
        {
            return FactValueType.DECIMAL;
        }


        public override void SetDefaultValue(T defaultValue)
        {
            this.defaultValue = (double)Convert.ChangeType(defaultValue, typeof(double));
        }

    }
}
