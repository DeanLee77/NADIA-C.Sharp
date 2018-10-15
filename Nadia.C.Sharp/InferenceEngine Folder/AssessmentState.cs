using System;
using System.Collections.Generic;
using System.Linq;
using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.NodeFolder;

namespace Nadia.C.Sharp.InferenceEngineFolder
{
    /*
     * HashMap<Object, FactValue> workingMemory
     *    first 'String' Key represents a Node's variableName and/or nodeName, or it could be a NodeSet's name in further development on. 
     *    second 'FactValue' Value represents the Rule's value or Fact's value.
     *    
     * List<String> inclusiveList
     *    it stores all relevant rule as assessment goes by, and the parameter String represents rule.getName()
     *    
     * List<String> exclusiveList
     *    it stores all irrelevant rule as assessment goes by, and the parameter String represents rule.getName()
     */
    public class AssessmentState<T>
    {
        
        private Dictionary<string, FactValue<T>> workingMemory;
        private List<string> inclusiveList;
        private List<string> exclusiveList;
        private List<string> summaryList;
        private List<string> mandatoryList;
        public AssessmentState()
        {
            this.workingMemory = new Dictionary<string, FactValue<T>>();
            this.inclusiveList = new List<string>();  // this is to capture all relevant rules 
            this.summaryList = new List<string>(); // this is to store all determined rules within assessment in order.
            this.exclusiveList = new List<string>();// this is to capture all trimmed rules 
            this.mandatoryList = new List<string>();
        }
        /*
         * this method is to get workingMemory 
         */
        public Dictionary<string, FactValue<T>> GetWorkingMemory()
        {
            return workingMemory;
        }
        /*
         * this method is to set workingMemory from RuleSet instance. this method has to be executed after AssessmentState object initialization.
         * all facts instance will be transfered from ruleMap in RuleSet instance to workingMemory in AssessmentState instance 
         */
        public void TransferFactMapToWorkingMemory(NodeSet<T> nodeSet)
        {
            if (!(nodeSet.GetFactMap().Count == 0))
            {
                this.workingMemory = nodeSet.TransferFactMapToWorkingMemory(workingMemory);
            }
        }
        /*
         * this is simply for setting workingMemory with a given workingMemory
         */
        public void SetWorkingMemory(Dictionary<string, FactValue<T>> workingMemory)
        {
            this.workingMemory = workingMemory;
        }
        /*
         * it allows a user to look up the workingMemory
         * @return FactValue
         */
        public FactValue<T> LookupWorkingMemory(string keyName)
        {
            return workingMemory[keyName];
        }

        /*
         * it is to get List<String> inclusiveList
         */
        public List<string> GetInclusiveList()
        {
            return inclusiveList;
        }
        public void SetInclusiveList(List<string> inclusiveList)
        {
            this.inclusiveList = inclusiveList;
        }
        public bool IsInclusiveList(string name)
        {
            bool isInTheList = false;
            if (this.inclusiveList.Contains(name))
            {
                isInTheList = true;
            }
            return isInTheList;
        }

        public void AddItemToSummaryList(string node)
        {
            if (!this.summaryList.Contains(node))
            {
                this.summaryList.Add(node);
            }
        }
        public List<string> GetSummaryList()
        {
            return summaryList;
        }
        public void SetSummaryList(List<string> summaryList)
        {
            this.summaryList = summaryList;
        }

        // exclusiveList
        public List<string> GetExclusiveList()
        {
            return exclusiveList;
        }
        public void SetExclusiveList(List<string> exclusiveList)
        {
            this.exclusiveList = exclusiveList;
        }
        public bool IsInExlusiveList(string name)
        {
            bool isInTheList = false;
            if (this.exclusiveList.Contains(name))
            {
                isInTheList = true;
            }
            return isInTheList;
        }

        //mandatoryList
        public List<string> GetMandatoryList()
        {
            return mandatoryList;
        }
        public void SetMandatoryList(List<string> mandatoryList)
        {
            this.mandatoryList = mandatoryList;
        }
        public void AddItemToMandatoryList(string nodeName)
        {
            if (!this.mandatoryList.Contains(nodeName))
            {
                this.mandatoryList.Add(nodeName);
            }
        }
        public bool IsInMandatoryList(string nodeName)
        {
            return this.mandatoryList.Contains(nodeName);
        }
        public bool AllMandatoryNodeDetermined()
        {
            return mandatoryList.All(nodeName => workingMemory.ContainsKey(nodeName));

        }
        /*
         * this method is to set a rule as a fact in the workingMemory 
         * before this method is called, nodeName should be given and look up nodeMap in NodeSet to find variableName of the node
         * then the variableName of the node should be passed to this method.
         */

        public void SetFact(string nodeVariableName, FactValue<T> value)
        {
            if (workingMemory.ContainsKey(nodeVariableName))
            {
                FactValue<T> tempFv = workingMemory[nodeVariableName];

                if (tempFv.GetFactValueType().Equals(FactValueType.LIST))
                {
                    ((FactListValue<T>)tempFv).AddFactValueToListValue(tempFv);
                }
                else
                {
                    FactListValue<T> flv = FactValue<T>.ParseList((T)Convert.ChangeType(new List<FactValue<T>>(), typeof(T)));
                    flv.AddFactValueToListValue(tempFv);
                }
            }
            else
            {
                workingMemory.Add(nodeVariableName, value);
            }
        }

        public FactValue<T> GetFact(string name)
        {
            return workingMemory[name];
        }

        public void RemoveFact(string name)
        {
            workingMemory.Remove(name);
        }
    }
}
