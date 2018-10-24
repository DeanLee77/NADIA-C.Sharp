using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactIntegerValue : FactValue 
    {
        private int value;

        public FactIntegerValue(int i)
        {
            SetValue(i);
        }

        public void SetValue(int value)
        {
            this.value = value;
        }

        public override FactValueType GetFactValueType()
        {
            return FactValueType.INTEGER;
        }

        public int GetValue()
        {
            return this.value;
        }
    }
}
