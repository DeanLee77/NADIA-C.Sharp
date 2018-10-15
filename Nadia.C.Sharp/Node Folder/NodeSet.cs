using System;
using System.Collections.Generic;
using System.Linq;
using Nadia.C.Sharp.FactValueFolder;

namespace Nadia.C.Sharp.NodeFolder
{
    public class NodeSet<T>
    {
        private string nodeSetName;
        private Dictionary<string, Node<T>> nodeMap ;
        private Dictionary<int?, string> nodeIdMap;
        private List<Node<T>> sortedNodeList;
        private Dictionary<string, FactValue<T>> inputMap;
        private Dictionary<string, FactValue<T>> factMap;
        private Node<T> defaultGoalNode;
        private DependencyMatrix dependencyMatrix;

        public NodeSet()
        {
            this.nodeSetName = "";
            this.inputMap = new Dictionary<string,FactValue<T>>();
            this.factMap = new Dictionary<string, FactValue<T>>();
            this.nodeMap = new Dictionary<string, Node<T>>();
            this.nodeIdMap = new Dictionary<int?, string>();
            this.sortedNodeList = new List<Node<T>>();

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
        public void SetNodeMap(Dictionary<string, Node<T>> nodeMap)
        {
            this.nodeMap = nodeMap;
        }
        public Dictionary<string, Node<T>> GetNodeMap()
        {
            return this.nodeMap;
        }
        
        public void SetNodeSortedList(List<Node<T>> sortedNodeList)
        {
            this.sortedNodeList = sortedNodeList;
        }
        public List<Node<T>> GetNodeSortedList()
        {
            return this.sortedNodeList;
        }
        
        public Dictionary<string, FactValue<T>> GetInputMap()
        {
            return this.inputMap;
        }
        
        public void SetFactMap(Dictionary<string, FactValue<T>> factMap)
        {
            this.factMap = factMap;
        }
        public Dictionary<string, FactValue<T>> GetFactMap()
        {
            return this.factMap;
        }
        
        public Node<T> GetNode(int nodeIndex)
        {
            return sortedNodeList[nodeIndex];
        }
        
        public Node<T> GetNode(string nodeName)
        {
            return nodeMap[nodeName];
        }
        
        public Node<T> GetNodeByNodeId(int nodeId)
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
        public Node<T> GetDefaultGoalNode()
        {
            return this.defaultGoalNode;
        }
        public Dictionary<string, FactValue<T>> TransferFactMapToWorkingMemory(Dictionary<string, FactValue<T>> workingMemory)
        {
            foreach(string str in inputMap.Keys)
            {
                workingMemory.Add(str, inputMap[str]);
            }
            
            return workingMemory;
        }
        
        


    }
}
