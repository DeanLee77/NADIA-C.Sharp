using System;
using System.Collections.Generic;
using System.Linq;
using Nadia.C.Sharp.FactValueFolder;

namespace Nadia.C.Sharp.NodeFolder
{
    public class NodeSet
    {
        private string nodeSetName;
        private Dictionary<string, Node> nodeMap ;
        private Dictionary<int?, string> nodeIdMap;
        private List<Node> sortedNodeList;
        private Dictionary<string, FactValue> inputMap;
        private Dictionary<string, FactValue> factMap;
        private Node defaultGoalNode;
        private DependencyMatrix dependencyMatrix;

        public NodeSet()
        {
            this.nodeSetName = "";
            this.inputMap = new Dictionary<string,FactValue>();
            this.factMap = new Dictionary<string, FactValue>();
            this.nodeMap = new Dictionary<string, Node>();
            this.nodeIdMap = new Dictionary<int?, string>();
            this.sortedNodeList = new List<Node>();

        }
        
        public DependencyMatrix GetDependencyMatrix()
        {
            return this.dependencyMatrix;
        }
        public void SetDependencyMatrix(DependencyMatrix dm)
        {
            this.dependencyMatrix = dm;
        }
        public void SetDependencyMatrix(int[,] dependencyMatrix)
        {
            this.dependencyMatrix = new DependencyMatrix(dependencyMatrix);
        }
        
        public String GetNodeSetName()
        {
            return this.nodeSetName;
        }
        public void SetNodeSetName(String nodeSetName)
        {
            this.nodeSetName = nodeSetName;
        }
        
        public void SetNodeIdMap(Dictionary<int?, string> nodeIdMap)
        {
            this.nodeIdMap = nodeIdMap;
        }
        public Dictionary<int?, string> GetNodeIdMap()
        {
            return this.nodeIdMap;
        }
        public void SetNodeMap(Dictionary<string, Node> nodeMap)
        {
            this.nodeMap = nodeMap;
        }
        public Dictionary<string, Node> GetNodeMap()
        {
            return this.nodeMap;
        }
        
        public void SetNodeSortedList(List<Node> sortedNodeList)
        {
            this.sortedNodeList = sortedNodeList;
        }
        public List<Node> GetNodeSortedList()
        {
            return this.sortedNodeList;
        }
        
        public Dictionary<string, FactValue> GetInputMap()
        {
            return this.inputMap;
        }
        
        public void SetFactMap(Dictionary<string, FactValue> factMap)
        {
            this.factMap = factMap;
        }
        public Dictionary<string, FactValue> GetFactMap()
        {
            return this.factMap;
        }
        
        public Node GetNode(int nodeIndex)
        {
            return sortedNodeList[nodeIndex];
        }
        
        public Node GetNode(string nodeName)
        {
            return nodeMap[nodeName];
        }
        
        public Node GetNodeByNodeId(int nodeId)
        {
            return GetNode(GetNodeIdMap()[nodeId]);
        }
        
         public int FindNodeIndex(string nodeName)
        {
            int nodeIndex = Enumerable.Range(0, GetNodeSortedList().Count).Where(i => GetNodeSortedList()[i].GetNodeName().Equals(nodeName)).ToArray()[0];

            return nodeIndex;
        }
        
        public void SetDefaultGoalNode(string name)
        {
            this.defaultGoalNode = this.nodeMap[name];
        }
        public Node GetDefaultGoalNode()
        {
            return this.defaultGoalNode;
        }
        public Dictionary<string, FactValue> TransferFactMapToWorkingMemory(Dictionary<string, FactValue> workingMemory)
        {
            foreach(string str in inputMap.Keys)
            {
                workingMemory.Add(str, inputMap[str]);
            }
            
            return workingMemory;
        }
        
        


    }
}
