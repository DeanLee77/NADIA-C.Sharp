using System;
namespace Nadia.C.Sharp.NodeFolder
{
    public class Dependency<T>
    {
        private int dependencyType; //this variable is to store 'AND/OR' DependencyType between Nodes
        private Node<T> parent; // this variable is to store a parent Node of this dependency
        private Node<T> child; // this variable is to store a child Node of this dependency

        //    public Dependency(Node child, String DependencyType)
        public Dependency(Node<T> parent, Node<T> child, int dependencyType)
        {
            this.parent = parent;
            this.child = child;
            this.dependencyType = dependencyType;
        }

        public Node<T> GetParentNode()
        {
            return parent;
        }
        public void SetParentNode(Node<T> parentNode)
        {
            this.parent = parentNode;
        }
        public Node<T> GetChildNode()
        {
            return child;
        }

        public int GetDependencyType()
        {
            return dependencyType;
        }

    }

}
