using System;
using System.Collections;
using System.Collections.Generic;

namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactListValue<T> : FactValue<T> //where T: IList
    {
        private T listValue;
        private FactValue<T> defaultValue;

        public FactListValue(T i)
        {
            SetListValue(i);
        }

        public void SetListValue(T listValue)
        {
            this.listValue = listValue;
        }

        public void AddFactValueToListValue(FactValue<T> fv)
        {
            (this.listValue as List<FactValue<T>>).Add(fv);
        }


        public override FactValueType GetFactValueType()
        {
            return FactValueType.LIST;
        }



        public override void SetDefaultValue(T defaultValue)
        {
            this.defaultValue = (FactValue<T>)Convert.ChangeType(defaultValue, typeof(FactValue<T>));
        }

        public override T GetValue()
        {
            return (T)Convert.ChangeType(this.listValue, typeof(T));
        }

        public override T GetDefaultValue()
        {

            return (T)Convert.ChangeType(this.defaultValue, typeof(T));
        }

    }
}
