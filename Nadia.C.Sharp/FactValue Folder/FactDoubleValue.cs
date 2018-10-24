using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactDoubleValue : FactValue
    {
        private double value;

        public FactDoubleValue(double d)
        {
            SetValue(d);
        }

        public void SetValue(double d)
        {
            this.value = d;
        }


        public double GetValue()
        {
            return this.value;
        }

        public override FactValueType GetFactValueType()
        {
            return FactValueType.DECIMAL;
        }

    }
}
