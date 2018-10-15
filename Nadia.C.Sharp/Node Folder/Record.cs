using System;
namespace Nadia.C.Sharp.NodeFolder
{
    public class Record
    {
        private string name;
        private string type;
        private int trueCount;
        private int falseCount;

        public Record()
        {
            name = "";
            type = "";
            trueCount = 0;
            falseCount = 0;
        }
        public Record(string name, string type, int trueCount, int falseCount)
        {
            this.name = name;
            this.type = type;
            this.trueCount = trueCount;
            this.falseCount = falseCount;
        }

        public void SetName(string name)
        {
            this.name = name;
        }
        public string GetName()
        {
            return this.name;
        }

        public void SetType(string type)
        {
            this.type = type;
        }
        public string GetRecordType()
        {
            return this.type;
        }

        public void SetTrueCount(int trueCount)
        {
            this.trueCount = trueCount;
        }
        public void addTrueCount(int trueCount)
        {
            this.trueCount += trueCount;
        }
        public void IncrementTrueCount()
        {
            this.trueCount++;
        }
        public int GetTrueCount()
        {
            return this.trueCount;
        }

        public void SetFalseCount(int falseCount)
        {
            this.falseCount = falseCount;
        }
        public void AddFalseCount(int falseCount)
        {
            this.falseCount += falseCount;
        }
        public void IncrementFalseCount()
        {
            this.falseCount++;
        }
        public int GetFalseCount()
        {
            return this.falseCount;
        }

    }
}
