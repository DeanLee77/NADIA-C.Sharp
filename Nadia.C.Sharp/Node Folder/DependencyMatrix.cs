using System;
using System.Collections.Generic;
using System.Linq;

namespace Nadia.C.Sharp.NodeFolder
{
    public class DependencyMatrix
    {

        /*
         *  order of dependency type
         *  1. MANDATORY
         *  2. OPTIONAL
         *  3. POSSIBLE
         *  4. AND
         *  5. OR
         *  6. NOT
         *  7. KNOWN
         *  
         *  int value will be '1' if any one of them is true case otherwise '0'
         *  for instance, if a rule is in 'MANDATORY AND NOT' dependency then 
         *  dependency type value is '1001010'
         *  
         *  if there is no dependency then value is 0000000
         */
        private int[,] dependencyMatrix;
        private int dependencyMatrixSize;

        public DependencyMatrix(int[,] dependencyMatrix)
        {
            this.dependencyMatrix = dependencyMatrix;
            this.dependencyMatrixSize = this.dependencyMatrix.Length/2;
        }

        public int[,] GetDependencyMatrixArray()
        {
            return this.dependencyMatrix;
        }

        public int GetDependencyType(int parentRuleId, int childRuleId)
        {
            return this.dependencyMatrix[parentRuleId, childRuleId];
        }


        public List<int> GetToChildDependencyList(int nodeId)
        {
            return Enumerable.Range(0, dependencyMatrixSize).Where(i => i != nodeId && this.dependencyMatrix[nodeId, i] != 0).ToList();
        }

        public List<int> GetOrToChildDependencyList(int nodeId)
        {
            int orDependency = DependencyType.GetOr();

            return Enumerable.Range(0, this.dependencyMatrixSize).Where(i => i != nodeId && (this.dependencyMatrix[nodeId, i] & orDependency) == orDependency).ToList();
        }

        public List<int> GetAndToChildDependencyList(int nodeId)
        {
            int andDependency = DependencyType.GetAnd();

            return Enumerable.Range(0, this.dependencyMatrixSize).Where(i => i != nodeId && (this.dependencyMatrix[nodeId, i] & andDependency) == andDependency).ToList();
        }

        public List<int> GetMandatoryToChildDependencyList(int nodeId)
        {
            int mandatoryDependency = DependencyType.GetMandatory();
            return Enumerable.Range(0, this.dependencyMatrixSize).Where(i => i != nodeId && (this.dependencyMatrix[nodeId, i] & mandatoryDependency) == mandatoryDependency).ToList();
        }


        public List<int> GetFromParentDependencyList(int nodeId)
        {
            return Enumerable.Range(0, this.dependencyMatrixSize).Where(i => i != nodeId && this.dependencyMatrix[i, nodeId] != 0).ToList();
        }

        public bool HasMandatoryChildNode(int nodeId)
        {
            return GetMandatoryToChildDependencyList(nodeId).Count() > 0;
        }

    }
}
