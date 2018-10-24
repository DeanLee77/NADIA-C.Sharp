using System;
using System.Collections.Generic;
using System.Linq;
using Nadia.C.Sharp.NodeFolder;

namespace Nadia.C.Sharp.InferenceEngineFolder
{
    public class TopoSort
    {
        /*
         *this topological sort method uses "Kahn's algorithm which is based on BFS(Breadth First Search)
         *within this method, the original 'dependencyMatrix' will lose information of dependency 
         *due to the reason that the algorithm itself uses the dependency information and delete it while topological sorting
         *Hence, this method needs to create copy of dependencyMatrix.
         */
        public static List<Node> BfsTopoSort(Dictionary<string, Node> nodeMap, Dictionary<int?, string> nodeIdMap, int[,] dependencyMatrix)
        {
            List<Node> sortedNodeList = new List<Node>();
            int sizeOfMatrix = dependencyMatrix.GetLength(1);
            int[,] copyOfDependencyMatrix = CreateCopyOfDependencyMatrix(dependencyMatrix, sizeOfMatrix);

            List<Node> tempList = new List<Node>();
            List<Node> SList = FillingSList(nodeMap, nodeIdMap, tempList, copyOfDependencyMatrix);


            while (SList.Count != 0)
            {
                Node node = SList[0];
                SList.RemoveAt(0);
                sortedNodeList.Add(node);
                int nodeId = node.GetNodeId();


                for (int i = 0; i < sizeOfMatrix; i++)
                {
                    if (nodeId != i && copyOfDependencyMatrix[nodeId, i] != 0)
                    {
                        copyOfDependencyMatrix[nodeId, i] = 0; // this is to remove dependency from 'node' to child node with nodeId == 'i'

                        int numberOfIncomingEdge = sizeOfMatrix - 1; // from this line, it is process to check whether or not the child node with nodeId == 'i' has any other incoming dependencies from other nodes, and the reason for subtracting 1 from matrixSize is to exclude node itself count from the matrix size.
                        for (int j = 0; j < sizeOfMatrix; j++)
                        {
                            if (j != i && copyOfDependencyMatrix[j, i] == 0)
                            {
                                numberOfIncomingEdge--;
                            }
                        }
                        if (numberOfIncomingEdge == 0) // there is no incoming dependencies for the node with nodeId == 'i'
                        {
                            SList.Add(nodeMap[nodeIdMap[i]]);
                        }
                    }

                }
                //          tempList.clear(); // tempList needs to be cleared because it is used for filling SList, and it is used to avoid creating List instance over and over again.
                //          SList = fillingSList(nodeMap, nodeIdMap, tempList, copyOfDependencyMatrix);
            }

            bool checkDAG = false;
            for (int i = 0; i < sizeOfMatrix; i++)
            {
                for (int j = 0; j < sizeOfMatrix; j++)
                {
                    if (i != j && copyOfDependencyMatrix[i, j] != 0)
                    {
                        checkDAG = true;
                        break;
                    }
                }
            }

            if (checkDAG)
            {
                sortedNodeList.Clear();
            }

            /*
             * if size of sortedNodeList is '0' then the graph is cyclic so that RuleSet needs rewriting due to it is in incorrect format
             */
            return sortedNodeList;


        }

        public static List<Node> FillingSList(Dictionary<string, Node> nodeMap, Dictionary<int?, string> nodeIdMap, List<Node> tempList, int[,] dependencyMatrix)
        {
            int sizeOfMatrix = dependencyMatrix.GetLength(1);

            for (int childRow = 0; childRow < sizeOfMatrix; childRow++)
            {
                int count = 0;
                for (int parentCol = 0; parentCol < sizeOfMatrix; parentCol++)
                {

                    if (dependencyMatrix[parentCol, childRow] == 0 && parentCol != childRow) // don't count when parentCol == childRow due to the reason that there shouldn't be value at the index. 
                    {
                        count++;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (count == sizeOfMatrix - 1) //exclude its own dependency 
                {
                    string tempNodeName = null;
                    nodeIdMap.TryGetValue(childRow, out tempNodeName);
                    if (tempNodeName != null)
                    {
                        tempList.Add(nodeMap[tempNodeName]);
                    }
                }
            }//initial 'S' List for Kahn's topological algorithm.

            return tempList;
        }

        public static int[,] CreateCopyOfDependencyMatrix(int[,] dependencyMatrix, int sizeOfMatrix)
        {

            int[,] copyOfDependencyMatrix = new int[sizeOfMatrix, sizeOfMatrix];
            for (int i = 0; i < sizeOfMatrix; i++)
            {
                for (int j = 0; j < sizeOfMatrix; j++)
                {
                    copyOfDependencyMatrix[i, j] = dependencyMatrix[i, j];
                }
            }

            return copyOfDependencyMatrix;
        }

        /*
         * this topological sort method uses DFS(Depth First Search)
         * At this point of time (10th Feb 2018), this method is strictly for sorting child nodes of IterateLine 
         * The reason for using this method is only for child nodes of IterateLine is that if there is a child node of local variable type
         * then the sorted order will NOT be appropriate to produce a next question.
         * 
         * For instance, if IterateLine node has a rule as following.
         * ------------------------------------------------------------
         * ALL service ITERATE: LIST OF service history
         *  AND number of services
         *  AND iterate rules
         *      OR one
         *          AND enlistment date >= 01/07/1951
         *          AND discharge date <= 6/12/1972
         *          AND NOT service type IS IN LIST: Special service
         *      OR two
         *          AND enlistment date >= 22/05/1986
         *          AND yearly period of service by 6/04/1994 >= 3
         *              AND yearly period of service by 6/04/1994 IS CALC (enlistment date - 6/04/1994)
         *                  NEEDS enlistment date
         *          AND NOT service type IS IN LIST: Special service
         *          AND discharge date >= 07/04/1994
         *          AND discharge date <= 30/06/2004
         *------------------------------------------------------------
         * and number of service is '2', then the sorted order will be as follows.
         * 
         * ------------------------------------------------------------
         *  ALL service ITERATE: LIST OF service history
         *  number of services
         *  1st service iterate rules
         *  2nd service iterate rules
         *  1st service one
         *  1st service two
         *  2nd service one
         *  2nd service two
         *  1st service enlistment date >= 01/07/1951
         *  1st service discharge date <= 6/12/1972
         *  1st service enlistment date >= 22/05/1986
         *  1st service yearly period of service by 6/04/1994 >= 3
         *  1st service service type IS IN LIST: Special service
         *  1st service enlistment date >= 07/04/1994
         *  1st service discharge date >= 30/06/2004
         *  2nd service enlistment date >= 01/07/1951
         *  2nd service discharge date <= 6/12/1972
         *  2nd service enlistment date >= 22/05/1986
         *  2nd service yearly period of service by 6/04/1994 >= 3
         *  2nd service service type IS IN LIST: Special service
         *  2nd service enlistment date >= 07/04/1994
         *  2nd service discharge date >= 30/06/2004
         *  1st service yearly period of service by 6/04/1994 IS CALC (enlistment date - 6/04/1994)
         *  2nd service yearly period of service by 6/04/1994 IS CALC (enlistment date - 6/04/1994)
         *  1st service enlistment date
         *  2nd service enlistment date
         *------------------------------------------------------------
         * And therefore, there would cause 1st service question and 2nd service question mixed
         * 
         *    !!!!!!!!!!!!!!   I M P O R T A N T  !!!!!!!!!!!!!!!!!!!!
         *    
         *    This method does NOT have a mechanism to check if it is DAG or not yet.
         *    
         */
        public static List<Node> DfsTopoSort(Dictionary<string, Node> nodeMap, Dictionary<int?, string> nodeIdMap, int[,] dependencyMatrix)
        {
            List<Node> sortedList = new List<Node>();

            int[,] copyOfDependencyMatrix = CreateCopyOfDependencyMatrix(dependencyMatrix, dependencyMatrix.GetLength(1));
            List<Node> S = FillingSList(nodeMap, nodeIdMap, new List<Node>(), copyOfDependencyMatrix);
            List<int?> visitedList = new List<int?>();
            while (S.Count != 0)
            {
                Node node = S[0];
                S.RemoveAt(0);

                sortedList.Add(node);
                visitedList.Add(node.GetNodeId());
                int nodeId = node.GetNodeId();

                List<int> childIdList = new List<int>();
                Enumerable.Range(0, copyOfDependencyMatrix.Length / 2).ToList().ForEach((i) =>
                {
                    if (copyOfDependencyMatrix[nodeId, i] != 0)
                    {
                        childIdList.Add(i);
                    }
                });
                childIdList.ForEach((id) =>
                {
                    Node currentNode = nodeMap[nodeIdMap[id]];
                    if (!visitedList.Contains(id))
                    {
                        sortedList.Add(currentNode);
                        visitedList.Add(id);
                    }
                    Deepening(nodeMap, nodeIdMap, copyOfDependencyMatrix, sortedList, visitedList, id);
                });

            }

            return sortedList;
        }

        public static void Deepening(Dictionary<string, Node> nodeMap, Dictionary<int?, string> nodeIdMap, int[,] dependencyMatrix, List<Node> sortedList, List<int?> visitedList, int childId)
        {
            List<int> childIdList = new List<int>();
            Enumerable.Range(0, dependencyMatrix.GetLength(1)).ToList().ForEach((i) =>
            {
                if (dependencyMatrix[childId, i] != 0)
                {
                    childIdList.Add(i);
                }
            });

            childIdList.ForEach((id) =>
            {
                Node currentNode = nodeMap[nodeIdMap[id]];
                if (!visitedList.Contains(id))
                {
                    sortedList.Add(currentNode);
                    visitedList.Add(id);
                }
                Deepening(nodeMap, nodeIdMap, dependencyMatrix, sortedList, visitedList, id);
            });

        }
        /*
         * this class is another version of topological sort.
         * the first version of topological sort used Kahn's algorithm which is based on Breadth First Search(BFS)
         * Topological sorted list is a fundamental part to get an order list of all questions.
         * However, it always provide same order at all times which might not be shortest path for a certain individual case therefore,
         * this topological sort based on historical record of each node/rule is suggested.
         * 
         * logic for the sorting is as follows; 
         * note: topological sort logic contains a recursive method 
         * 1. set 'S' and 'sortedList'
         * 2. get all data for each rules from database as a HashMap<String, Record>
         * 3. find rules don't have any parent rules, and add them into 'S' list
         * 4. if there is an element in the 'S' list
         * 5. visit the element
         *    5.1 if the element has any child rules
         *        5.1.1 get a list of all child rules, and keep visiting until there are no non-visited rules
         *        5.1.2 if there is not any 'OR' rules ( there are only 'AND' rules)
         *              5.1.2.1 find the most negative rule, and add the rule into the 'sortedList'
         *        5.1.3 if there is not any 'AND' rules ( there are only 'OR' rules)
         *              5.1.3.1 find the most positive rule, and add the rule into the 'sortedList'
         * 
         */

        public static List<Node> DfsTopoSort(Dictionary<string, Node> nodeMap, Dictionary<int?, string> nodeIdMap, int[,] dependencyMatrix, Dictionary<string, Record> recordMapOfNodes)
        {

            List<Node> sortedList = new List<Node>();

            if (recordMapOfNodes == null || recordMapOfNodes.Count == 0)
            {
                sortedList = BfsTopoSort(nodeMap, nodeIdMap, dependencyMatrix);
            }
            else
            {
                List<Node> visitedNodeList = new List<Node>();
                int[,] copyOfDependencyMatrix = CreateCopyOfDependencyMatrix(dependencyMatrix, dependencyMatrix.GetLength(1));

                List<Node> S = FillingSList(nodeMap, nodeIdMap, new List<Node>(), copyOfDependencyMatrix);

                while (S.Count != 0)
                {
                    Node node = S[0];
                    S.RemoveAt(0);
                    visitedNodeList.Add(node);
                    Visit(node, sortedList, recordMapOfNodes, nodeMap, nodeIdMap, visitedNodeList, dependencyMatrix);
                }
            }

            return sortedList;
        }

        /*
         * The idea of this method is to visit a rule that could get a result of parent rule of the rule as quick as it can be
         * for instance, if a 'OR' child rule is 'TRUE' then the parent rule is 'TRUE', 
         * and if a 'AND' child rule is 'FALSE' then the parent rule is 'FALSE'. 
         * AS result, visit more likely true 'OR' rule or more likely false 'AND' rule to determine a parent rule as fast as we can
         */
        public static List<Node> Visit(Node node, List<Node> sortedList, Dictionary<string, Record> recordMapOfNodes, Dictionary<string, Node> nodeMap, Dictionary<int?, string> nodeIdMap, List<Node> visitedNodeList, int[,] dependencyMatrix)
        {
            if (node != null)
            {
                sortedList.Add(node);
                int nodeId = node.GetNodeId();
                int orDependencyType = DependencyType.GetOr();
                int andDependencyType = DependencyType.GetAnd();
                List<int> dependencyMatrixAsList = new List<int>();
                Enumerable.Range(0, dependencyMatrix.GetLength(1)).ToList().ForEach((index) => dependencyMatrixAsList.Add(dependencyMatrix[nodeId, index]));
                List<int> orOutDependency = (System.Collections.Generic.List<int>)Enumerable.Range(0, dependencyMatrixAsList.Count)
                                                                                            .Where(index => (dependencyMatrixAsList[index] & orDependencyType) == orDependencyType);
                    
                List<int> andOutDependency = (System.Collections.Generic.List<int>)Enumerable.Range(0, dependencyMatrixAsList.Count)
                                                                                             .Where(index => (dependencyMatrixAsList[index] & andDependencyType) == andDependencyType);
                   

                if (orOutDependency.Count != 0 || andOutDependency.Count != 0)
                {
                    List<Node> childRuleList = new List<Node>();
                    Enumerable.Range(0, dependencyMatrixAsList.Count).ToList()
                              .Where(childIndex => dependencyMatrixAsList[childIndex] != 0)
                              .ToList()
                              .ForEach(item => childRuleList.Add(nodeMap[nodeIdMap[item]]));


                    if (orOutDependency.Count != 0 && andOutDependency.Count == 0)
                    {
                        while (childRuleList.Count != 0)
                        {
                            /* 
                             * the reason for selecting an option having more number of 'yes' is as follows
                             * if it is 'OR' rule and it is 'TRUE' then it is the shortest path, and ignore other 'OR' rules
                             * Therefore, looking for more likely 'TRUE' rule would be the shortest one rather than
                             * looking for more likely 'FALSE' rule in terms of processing time
                             */
                            Node theMostPositive = FindTheMostPositive(childRuleList, recordMapOfNodes, dependencyMatrixAsList);
                            //                              Node theMostPositive = findTheMostPositive(childRuleList, recordMapOfNodes);
                            if (!visitedNodeList.Contains(theMostPositive))
                            {
                                visitedNodeList.Add(theMostPositive);
                                sortedList = Visit(theMostPositive, sortedList, recordMapOfNodes, nodeMap, nodeIdMap, visitedNodeList, dependencyMatrix);
                            }
                        }

                    }
                    else
                    {
                        if (orOutDependency.Count == 0 && andOutDependency.Count != 0)
                        {
                            /* 
                             * the reason for selecting an option having more number of 'yes' is as follows
                             * if it is 'AND' rule and it is 'FALSE' then it is the shortest path, and ignore other 'AND' rules
                             * Therefore, looking for more likely 'FALSE' rule would be the shortest one rather than
                             * looking for more likely 'TRUE' rule in terms of processing time
                             */
                            while (childRuleList.Count != 0)
                            {
                                Node theMostNegative = FindTheMostNegative(childRuleList, recordMapOfNodes, dependencyMatrixAsList);
                                //                                  Node theMostNegative = findTheMostNegative(childRuleList, recordMapOfNodes);
                                if (!visitedNodeList.Contains(theMostNegative))
                                {
                                    visitedNodeList.Add(theMostNegative);
                                    sortedList = Visit(theMostNegative, sortedList, recordMapOfNodes, nodeMap, nodeIdMap, visitedNodeList, dependencyMatrix);
                                }
                            }
                        }
                    }
                }
            }


            return sortedList;
        }

        public static Node FindTheMostPositive(List<Node> childNodeList, Dictionary<string, Record> recordListOfNodes, List<int> dependencyMatrixAsList)
        {
            Node theMostPositive = null;
            int yesCount = 0;
            int noCount = 0;
            float theMostPossibility = 0;
            int sum = 0;
            float result = 0;

            foreach (Node node in childNodeList)
            {
                string prefix = "";
                int? dependencyType = dependencyMatrixAsList[node.GetNodeId()];
                if ((dependencyType & DependencyType.GetKnown()) == DependencyType.GetKnown())
                {
                    prefix = "known ";
                }
                else if ((dependencyType & DependencyType.GetNot()) == DependencyType.GetNot())
                {
                    prefix = "not ";
                }
                else if ((dependencyType & (DependencyType.GetNot() | DependencyType.GetKnown())) == (DependencyType.GetNot() | DependencyType.GetKnown()))
                {
                    prefix = "not known ";
                }

                Record recordOfNode = recordListOfNodes[prefix + node.GetNodeName()];
                yesCount = recordOfNode != null ? recordOfNode.GetTrueCount() : 0;
                noCount = recordOfNode != null ? recordOfNode.GetFalseCount() : 0;
                int yesPlusNoCount = (yesCount + noCount) == 0 ? -1 : (yesCount + noCount);

                result = (float)yesCount / yesPlusNoCount;
                if (Analysis(result, theMostPossibility, yesPlusNoCount, sum))
                {
                    theMostPossibility = result;
                    sum = yesCount + noCount;
                    theMostPositive = node;
                }
            }
            childNodeList.Remove(theMostPositive);
            return theMostPositive;
        }

        public static Node FindTheMostPositive(List<Node> childNodeList, Dictionary<string, Record> recordListOfNodes)
        {
            Node theMostPositive = null;
            int yesCount = 0;
            int noCount = 0;
            float theMostPossibility = 0;
            int sum = 0;
            float result = 0;

            foreach (Node node in childNodeList)
            {
                Record recordOfNode = recordListOfNodes[node.GetNodeName()];
                yesCount = recordOfNode != null ? recordOfNode.GetTrueCount() : 0;
                noCount = recordOfNode != null ? recordOfNode.GetFalseCount() : 0;
                int yesPlusNoCount = (yesCount + noCount) == 0 ? -1 : (yesCount + noCount);

                result = (float)yesCount / yesPlusNoCount;
                if (Analysis(result, theMostPossibility, yesPlusNoCount, sum))
                {
                    theMostPossibility = result;
                    sum = yesCount + noCount;
                    theMostPositive = node;
                }
            }
            childNodeList.Remove(theMostPositive);
            return theMostPositive;

        }

        public static Node FindTheMostNegative(List<Node> childNodeList, Dictionary<string, Record> recordListOfNodes, List<int> dependencyMatrixAsList)
        {
            Node theMostNegative = null;
            int yesCount = 0;
            int noCount = 0;
            float theMostPossibility = 0;
            int sum = 0;
            float result = 0;

            foreach (Node node in childNodeList)
            {
                string prefix = "";
                int dependencyType = dependencyMatrixAsList[node.GetNodeId()];
                if ((dependencyType & DependencyType.GetKnown()) == DependencyType.GetKnown())
                {
                    prefix = "known ";
                }
                else if ((dependencyType & DependencyType.GetNot()) == DependencyType.GetNot())
                {
                    prefix = "not ";
                }
                else if ((dependencyType & (DependencyType.GetNot() | DependencyType.GetKnown())) == (DependencyType.GetNot() | DependencyType.GetKnown()))
                {
                    prefix = "not known ";
                }

                Record recordOfNode = recordListOfNodes[prefix + node.GetNodeName()];
                yesCount = recordOfNode != null ? recordOfNode.GetTrueCount() : 0;
                noCount = recordOfNode != null ? recordOfNode.GetFalseCount() : 0;

                int yesPlusNoCount = (yesCount + noCount) == 0 ? -1 : (yesCount + noCount);
                result = (float)noCount / yesPlusNoCount;

                if (Analysis(result, theMostPossibility, yesPlusNoCount, sum))
                {
                    theMostPossibility = result;
                    sum = yesPlusNoCount == -1 ? yesPlusNoCount : yesCount + noCount;
                    theMostNegative = node;
                }


            }
            childNodeList.Remove(theMostNegative);
            return theMostNegative;
        }

        public static Node FindTheMostNegative(List<Node> childNodeList, Dictionary<string, Record> recordListOfNodes)
        {
            Node theMostNegative = null;
            int yesCount = 0;
            int noCount = 0;
            float theMostPossibility = 0;
            int sum = 0;
            float result = 0;

            foreach (Node node in childNodeList)
            {

                Record recordOfNode = recordListOfNodes[node.GetNodeName()];
                yesCount = recordOfNode != null ? recordOfNode.GetTrueCount() : 0;
                noCount = recordOfNode != null ? recordOfNode.GetFalseCount() : 0;

                int yesPlusNoCount = (yesCount + noCount) == 0 ? -1 : (yesCount + noCount);
                result = (float)noCount / yesPlusNoCount;

                if (Analysis(result, theMostPossibility, yesPlusNoCount, sum))
                {
                    theMostPossibility = result;
                    sum = yesPlusNoCount == -1 ? yesPlusNoCount : yesCount + noCount;
                    theMostNegative = node;
                }


            }
            childNodeList.Remove(theMostNegative);
            return theMostNegative;
        }

        public static bool Analysis(float result, float theMostPossibility, int yesCountNoCount, int sum)
        {
            bool highlyPossible = false;
            /*
             * firstly select an option having more cases and high possibility
             */
            if (result > theMostPossibility && yesCountNoCount >= sum)
            {
                highlyPossible = true;
            }
            /*
                 * secondly, even though the number of being used case is fewer, and it has a high possibility
                 * then still select the option
                 */
            else if (result >= theMostPossibility && result == 0 && theMostPossibility == 0 && yesCountNoCount > sum)
            {
                highlyPossible = true;
            }
            else if (result >= theMostPossibility && result == 0 && yesCountNoCount == -1 && sum <= 0 && sum != -1)
            {
                highlyPossible = true;
            }

            return highlyPossible;
        }

    }
}
