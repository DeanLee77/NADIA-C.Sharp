using System;
using System.Collections.Generic;

namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactListValue : FactValue //where T: IList
    {
        private List<FactValue> listValue;
        private FactValue defaultValue;

        public FactListValue(List<FactValue> i)
        {
            SetValue(i);
        }

        public void SetValue(List<FactValue> listValue)
        {
            this.listValue = listValue;
        }

        public void AddFactValueToListValue(FactValue fv)
        {
            this.listValue.Add(fv);
        }

        public override FactValueType GetFactValueType()
        {
            return FactValueType.LIST;
        }

        public void SetDefaultValue(FactValue defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        public List<FactValue> GetValue()
        {
            return this.listValue;
        }

        public FactValue GetDefaultValue()
        {

            return this.defaultValue;
        }

    }
}
