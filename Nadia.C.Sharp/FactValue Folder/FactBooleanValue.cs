using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactBooleanValue : FactValue 
    {
        private bool value;

        public FactBooleanValue(bool booleanValue)
        {
            SetValue(booleanValue);
        }

        public FactBooleanValue(FactValue fv)
        {
            this.value = Boolean.Parse(FactValue.GetValueInString(FactValueType.BOOLEAN, fv));
        }


        public void SetValue(bool booleanValue)
        {
            this.value = booleanValue;
        }

        public FactBooleanValue NegatingValue()
        {
            return new FactBooleanValue(!this.value);
        }

        public override FactValueType GetFactValueType()
        {
            return FactValueType.BOOLEAN;
        }

        public bool GetValue()
        {
            return this.value;
        }
    }
}
