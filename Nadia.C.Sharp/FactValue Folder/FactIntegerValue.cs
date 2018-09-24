using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactIntegerValue<T> : FactValue<T>
    {
        private int value;
        private int? defaultValue = null; //int? means nullable int primative type

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


        public override void SetDefaultValue(T defaultValue)
        {
            this.defaultValue = (int)Convert.ChangeType(defaultValue, typeof(int));
        }


        public override T GetValue()
        {
            return (T)Convert.ChangeType(this.value, typeof(T));
        }



        public override T GetDefaultValue()
        {
            // TODO Auto-generated method stub
            return (T)Convert.ChangeType(this.defaultValue, typeof(T));
        }

    }
}
