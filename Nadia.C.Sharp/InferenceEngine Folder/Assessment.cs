using System;
using Nadia.C.Sharp.NodeFolder;

namespace Nadia.C.Sharp.InferenceEngineFolder
{
    /*
    the reason of having Assessment class is to allows a user to do multiple assessment within one or multiple conditions.
    */
    public class Assessment<T>
    {
        private Node<T> goalNode;
        private int goalNodeIndex; // the goal rule index in ruleList of ruleSet
        /*
         * each instance of this object has a variable of ruleToBeAsked due to the following reasons;
         * 1. a user will be allowed to do assessment on multiple investigation points at the same time;
         * 2. a user will be allowed to do an assessment within another assessment. 
         */
        private Node<T> nodeToBeAsked;
        
        /*
         * this variable is to track next node to be asked within 'IterateLine' type node.
         * However, better way needs to be found.
         */
        private Node<T> auxNodeToBeAsked;
        
        public Assessment(NodeSet<T> ns, string goalNodeName)
        {
            goalNode = ns.GetNodeMap()[goalNodeName];
            goalNodeIndex = ns.FindNodeIndex(goalNodeName);
            nodeToBeAsked = null;
            auxNodeToBeAsked = null;

        }

        public void SetAssessment(NodeSet<T> nodeSet, string goalNodeName)
        {
            goalNode = nodeSet.GetNodeMap()[goalNodeName];
            goalNodeIndex = nodeSet.FindNodeIndex(goalNodeName);
            nodeToBeAsked = null; 
        }

        public Node<T> GetGoalNode()
        {
            return this.goalNode;
        }
        public int GetGoalNodeIndex()
        {
            return this.goalNodeIndex;
        }
        
        public void SetNodeToBeAsked(Node<T> nodeToBeAsked)
        {
            this.nodeToBeAsked = nodeToBeAsked;
        }
        public Node<T> GetNodeToBeAsked()
        {
            return this.nodeToBeAsked;
        }
         
        public void SetAuxNodeToBeAsked(Node<T> auxNodeToBeAsked)
        {
            this.auxNodeToBeAsked = auxNodeToBeAsked;
        }

        public Node<T> GetAuxNodeToBeAsked()
        {
            return this.auxNodeToBeAsked;
        }
    
    }
}
