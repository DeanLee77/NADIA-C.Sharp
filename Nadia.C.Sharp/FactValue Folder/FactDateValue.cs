using System;
using System.Text.RegularExpressions;

namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactDateValue : FactValue
    {
        private DateTime value;

        public FactDateValue(DateTime date)
        {
            SetValue(date);
        }
        //public FactDateValue(FactValue fv)
        //{
        //    string[] dateTimeInString = Regex.Split(Regex.Split(FactValue.GetValueInString(FactValueType.DATE, )))
        //    this.value = new DateTime
        //}

        public void SetValue(DateTime cal)
        {
            this.value = cal;
        }

        public DateTime GetValue()
        {
            return this.value;
        }

        public override FactValueType GetFactValueType()
        {
            return FactValueType.DATE;
        }

    }
}
