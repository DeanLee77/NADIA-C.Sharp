using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactDateValue<T> : FactValue<T>
    {
        private DateTime value;
        private DateTime defaultValue;

        public FactDateValue(DateTime date)
        {
            SetValue(date);
        }

        public void SetValue(DateTime cal)
        {
            this.value = cal;
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
            return FactValueType.DATE;
        }

        public override void SetDefaultValue(T defaultValue)
        {
            this.defaultValue = (DateTime)Convert.ChangeType(defaultValue, typeof(DateTime));
        }

    }
}
