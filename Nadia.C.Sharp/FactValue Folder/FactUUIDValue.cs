using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactUUIDValue<T> : FactValue<T>
    {
        private String value;
        private String defaultValue;

        public FactUUIDValue(String uuid)
        {
            SetValue(uuid);
        }



        public void SetValue(String uuid)
        {
            this.value = uuid;
        }



        public override FactValueType GetFactValueType()
        {
            return FactValueType.UUID;
        }


        public override void SetDefaultValue(T defaultValue)
        {
            this.defaultValue = (string)Convert.ChangeType(defaultValue, typeof(string));
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
