using System;
namespace Nadia.C.Sharp.FactValueFolder
{
    public class FactUUIDValue : FactValue
    {
        private string value;

        public FactUUIDValue(string uuid)
        {
            SetValue(uuid);
        }

        public void SetValue(string uuid)
        {
            this.value = uuid;
        }

        public override FactValueType GetFactValueType()
        {
            return FactValueType.UUID;
        }

        public string GetValue()
        {
            return this.value;
        }

    }
}
