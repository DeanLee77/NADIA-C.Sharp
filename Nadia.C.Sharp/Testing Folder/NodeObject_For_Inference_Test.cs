using System;
namespace Nadia.C.Sharp.TestingFolder
{
    public class NodeObject_For_Inference_Test
    {

        private string name;
        private string[] valueArray;
        private string value;

        public NodeObject_For_Inference_Test(string name, string[] valueArray)
        {
            this.name = name;
            this.valueArray = valueArray;
        }

        public string GetName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public string[] GetValueArray()
        {
            return valueArray;
        }

        public void setValueArray(string[] valueArray)
        {
            this.valueArray = valueArray;
        }

        public string GetValue()
        {
            Random rand = new Random();
            this.value = this.valueArray[rand.Next(2)];
            return value;
        }

    }
}
