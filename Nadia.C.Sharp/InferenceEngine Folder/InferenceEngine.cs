using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.NodeFolder;
using Newtonsoft.Json.Linq;

namespace Nadia.C.Sharp.InferenceEngineFolder
{
    public class InferenceEngine<T>
    {
        private NodeSet<T> nodeSet;
        private Node<T> targetNode;
        private AssessmentState<T> ast;
        private Assessment<T> ass;
        private List<Node<T>> nodeFactList;
        private Jint.Engine scriptEngine = new Jint.Engine();




        //    private int ruleIndex = 0;
        public InferenceEngine()
        {

        }
        public InferenceEngine(NodeSet<T> nodeSet)
        {
            this.nodeSet = nodeSet;
            ast = NewAssessmentState();
            Dictionary<string, FactValue<T>> tempFactMap = nodeSet.GetFactMap();
            Dictionary<string, FactValue<T>> tempWorkingMemory = ast.GetWorkingMemory();

            if (!(tempFactMap.Count == 0))
            {
                tempFactMap.Keys.ToList().ForEach((key) => tempWorkingMemory.Add(key, tempFactMap[key])); 
            }
            nodeFactList = new List<Node<T>>(nodeSet.GetNodeSortedList().Count * 2); // contains all rules set as a fact given by a user from a ruleList

        }

        public void AddNodeSet(NodeSet<T> nodeSet2)
        {

        }

        public void SetNodeSet(NodeSet<T> nodeSet)
        {
            this.nodeSet = nodeSet;
            ast = NewAssessmentState();
            Dictionary<string, FactValue<T>> tempFactMap = nodeSet.GetFactMap();
            Dictionary<string, FactValue<T>> tempWorkingMemory = ast.GetWorkingMemory();

            if (!(tempFactMap.Count == 0))
            {
                tempFactMap.Keys.ToList().ForEach((key) => tempWorkingMemory.Add(key, tempFactMap[key])); 
            }
            nodeFactList = new List<Node<T>>(nodeSet.GetNodeSortedList().Count * 2); // contains all rules set as a fact given by a user from a ruleList

        }

        public NodeSet<T> GetNodeSet()
        {
            return this.nodeSet;
        }

        public AssessmentState<T> GetAssessmentState()
        {
            return this.ast;
        }

        public AssessmentState<T> NewAssessmentState()
        {
            int initialSize = nodeSet.GetNodeSortedList().Count() * 2;
            AssessmentState<T> ast = new AssessmentState<T>();
            List<string> inclusiveList = new List<string>(initialSize);
            List<string> summaryList = new List<string>(initialSize);
            ast.SetInclusiveList(inclusiveList);
            ast.SetSummaryList(summaryList);

            return ast;
        }

        public void SetAssessment(Assessment<T> ass)
        {
            this.ass = ass;
        }

        public Assessment<T> GetAssessment()
        {
            return this.ass;
        }

        public Jint.Engine GetScriptEngine()
        {
            return this.scriptEngine;
        }

        /*
         * this method is to extract all variableName of Nodes, and put them into a List<String>
         * it may be useful to display and ask a user to select which information they do have even before starting Inference process
         */
        public List<string> GetListOfVariableNameAndValueOfNodes()
        {
            List<string> variableNameList = new List<string>();
            nodeSet.GetNodeMap().Values.ToList().ForEach(node => {
                variableNameList.Add(node.GetVariableName());
                FactValueType nodeFactValueType = node.GetFactValue().GetFactValueType();
                if (nodeFactValueType.Equals(FactValueType.BOOLEAN) || nodeFactValueType.Equals(FactValueType.STRING))
                {
                    variableNameList.Add(node.GetFactValue().GetValue().ToString());
                }
            });


            return variableNameList;
        }
        /*
         * this method allows to store all information via GUI even before starting Inference process. 
         */
        public void AddNodeFact(string nodeVariableName, FactValue<T> fv)
        {
            nodeSet.GetNodeMap().Values.ToList().ForEach((node) =>
            {
                if (node.GetVariableName().Equals(nodeVariableName) || node.GetFactValue().GetValue().ToString().Equals(nodeVariableName))
                {
                    nodeFactList.Add(node);
                }
            });
               
            ast.GetWorkingMemory().Add(nodeVariableName, fv);

        }

        /*
         * this method is to find all relevant Nodes(immediate child nodes of the most parent) with given information from a user
         * while finding out all relevant factors, all given information will be stored in AssessmentState.workingMemory
         */
        public List<Node<T>> FindRelevantFactors()
        {
            List<Node<T>> relevantFactorList = new List<Node<T>>();

            if (nodeFactList.Count != 0)
            {
                nodeFactList.ForEach((node) => {
                    if (nodeSet.GetDependencyMatrix().GetFromParentDependencyList(node.GetNodeId()).Count != 0)
                    {
                        Node<T> relevantNode = AuxFindRelevantFactors(node);
                        relevantFactorList.Add(relevantNode);
                    }
                });
            }
            return relevantFactorList;
        }

        public Node<T> AuxFindRelevantFactors(Node<T> node)
        {
            Node<T> relevantFactorNode = null;
            List<int> incomingDependencyList = nodeSet.GetDependencyMatrix().GetFromParentDependencyList(node.GetNodeId()); // it contains all id of parent node where dependency come from
            if (incomingDependencyList.Count != 0)
            {
                for (int i = 0; i < incomingDependencyList.Count; i++)
                {
                    Node<T> parentNode = nodeSet.GetNodeMap()[nodeSet.GetNodeIdMap()[incomingDependencyList[i]]];
                  if (nodeSet.GetDependencyMatrix().GetFromParentDependencyList(parentNode.GetNodeId()).Count!= 0
                            && !parentNode.GetNodeName().Equals(nodeSet.GetNodeSortedList()[0].GetNodeName()))
                    {
                        relevantFactorNode = AuxFindRelevantFactors(parentNode);

                    }
                }

            }
            return relevantFactorNode;
        }

        /*
         * this method uses 'BACKWARD-CHAININING', and it will return node to be asked of a given assessment, which has not been determined and 
         * does not have any child nodes if the goal node of the given assessment has still not been determined.
         */
        public Node<T> GetNextQuestion(Assessment<T> ass)
        {
            if (!ast.GetInclusiveList().Contains(ass.GetGoalNode().GetNodeName()))
            {
                ast.GetInclusiveList().Add(ass.GetGoalNode().GetNodeName());
            }

            /*
             * Default goal rule of a rule set which is a parameter of InferenceEngine will be evaluated by forwardChaining when any rule is evaluated within the rule set
             */
            if (ast.GetWorkingMemory()[ass.GetGoalNode().GetNodeName()] == null || !ast.AllMandatoryNodeDetermined())
            {
                for (int i = ass.GetGoalNodeIndex(); i < nodeSet.GetNodeSortedList().Count; i++)
                {
                    Node<T> node = nodeSet.GetNodeSortedList()[i];


                    /*
                     * Step1. does the rule currently being been checked have child rules && not yet evaluated && is in the inclusiveList?
                     *     if no then ask a user to evaluate the rule, 
                     *                and do back propagating with a result of the evaluation (note that this part will be handled in feedAnswer())
                     *     if yes then move on to following step
                     *     
                     * Step2. does the rule currently being been checked have child rules? 
                     *     if yes then add the child rules into the inclusiveList
                     */
                    int nodeId = node.GetNodeId();
                    if (i != ass.GetGoalNodeIndex())
                    {
                        List<int> parentDependencyList = nodeSet.GetDependencyMatrix().GetFromParentDependencyList(nodeId);
                        if (parentDependencyList.Count != 0)
                        {

                            parentDependencyList.ToList().ForEach((parentId) =>{
                                if((nodeSet.GetDependencyMatrix().GetDependencyType(parentId, nodeId) & DependencyType.GetMandatory()) == DependencyType.GetMandatory()
                                    && !ast.IsInclusiveList(node.GetNodeName())
                                    && !IsIterateLineChild(node.GetNodeId()))    
                                {
                                    ast.AddItemToMandatoryList(node.GetNodeName());
                                }
                            });
                        }
                    }

                    if(nodeId != ass.GetGoalNode().GetNodeId() && node.GetLineType().Equals(LineType.ITERATE) && !ast.GetWorkingMemory().ContainsKey(node.GetNodeName()))
                    {   
                        FactValue<T> givenListNameFv = this.ast.GetWorkingMemory()[(node as IterateLine<T>).GetGivenListName()];
                        String givenListName = "";
                        if(givenListNameFv != null)
                        {
                            givenListName = givenListNameFv.ToString().Trim();
                        }
                        if(givenListName.Length > 0)
                        {
                            (node as IterateLine<T>).IterateFeedAnswers(givenListName, this.nodeSet, this.ast, ass);
                        }
                        else
                        {
                            if(!this.ast.GetWorkingMemory().ContainsKey(node.GetNodeName()) && !this.ast.GetExclusiveList().Contains(node.GetNodeName()))
                            {
                                ass.SetNodeToBeAsked(node);
                                int indexOfRuleToBeAsked = i;

                                return (node as IterateLine<T>).GetIterateNextQuestion(this.nodeSet, this.ast);
                            }
                        }                           
                    }
                    else if(!HasChildren(nodeId) && ast.GetInclusiveList().Contains(node.GetNodeName())
                            && !CanEvaluate(node))
                    {
                        ass.SetNodeToBeAsked(node);
                        int indexOfRuleToBeAsked = i;

                        return ass.GetNodeToBeAsked();
                    }
                    else if(HasChildren(nodeId) && !ast.GetWorkingMemory().ContainsKey(node.GetVariableName())
                            && !ast.GetWorkingMemory().ContainsKey(node.GetNodeName()) && ast.GetInclusiveList().Contains(node.GetNodeName()))
                    {
                        AddChildRuleIntoInclusiveList(node);
                    }
               }
            }   
            return ass.GetNodeToBeAsked();
        }
    
    
        public List<string> GetQuestionsFromNodeToBeAsked(Node<T> nodeToBeAsked)
        {
            List<string> questionList = new List<string>();
            LineType lineTypeOfNodeToBeAsked = nodeToBeAsked.GetLineType();
            // the most child node line types are as follows
            // ValueConclusionLine type
            if (lineTypeOfNodeToBeAsked.Equals(LineType.VALUE_CONCLUSION))
            {
                /*
                 * if the line format is 'A -statement' then node's nodeName and variableName has same value so that either of them can be asked as a question
                 * 
                 * if the line format is 'A IS IN LIST B' then the value of node's variableName is 'A' and the value of node's value is 'B' so that only 'A' needs to be asked.
                 * list 'B' has to be provided in 'FIXED' list
                 * 
                 * In conclusion, if the line type is 'ValueConclusionLine' then node's variableName should be asked regardless its format
                 */

                questionList.Add(nodeToBeAsked.GetVariableName());
            }
            // ComparionLine type
            else if (lineTypeOfNodeToBeAsked.Equals(LineType.COMPARISON))
            {
                if(!this.ast.GetWorkingMemory().ContainsKey(((ComparisonLine<T>)nodeToBeAsked).GetLHS()))
                {
                    questionList.Add(((ComparisonLine<T>)nodeToBeAsked).GetLHS());    
                }


                if (!TypeAlreadySet(nodeToBeAsked.GetFactValue()) && !this.ast.GetWorkingMemory().ContainsKey(((ComparisonLine<T>)nodeToBeAsked).GetRHS().GetValue().ToString()))
                {
                    questionList.Add(nodeToBeAsked.GetFactValue().GetValue().ToString());
                }
            }

            questionList.ForEach((item) => ast.GetInclusiveList().Add(item));
            
            return questionList;
        }


        public Dictionary<string, FactValueType> FindTypeOfElementToBeAsked(Node<T> node)
        {
            /*
             * FactValueType can be handled as of 16/06/2017 is as follows;
             *  1. TEXT, STRING;
             *  2. INTEGER, NUMBER;
             *  3. DOUBLE, DECIMAL;
             *  4. BOOLEAN;
             *  5. DATE;
             *  6. HASH;
             *  7. UUID; and
             *  8. URL.   
             * rest of them (LIST, RULE, RULE_SET, OBJECT, UNKNOWN, NULL) can't be handled at this stage
             */
            FactValueType fvt = FactValueType.BOOLEAN;
            Dictionary<string, FactValueType> factValueTypeMap = new Dictionary<string,FactValueType>();
            /*
             * In a case of that if type of toBeAsked node is ComparisonLine type with following conditions;
             *    - the type of the node's variable to compare is already set as 
             *      DefiString (eg. 'dean' or "dean"), Integer (eg. 1, or 2), Double (eg. 1.2 or 2.1), Date (eg. 21/3/1299), Hash, UUID, or URL 
             *   then don't need to look into InputMap or FactMap to check the element's type of 'toBeAsked node' 
             *   simply because we can check by looking at type of value variable because two different type CANNOT be compared
             *   
             * If neither type of variable is NOT defined in INPUT/FACT list nor the above case, and value of nodeVariable is same as value of nodeValueString  
             * then the engine will recognise a nodeVariable or/and nodeVlaue as a boolean type 
             * so that the question for a nodeVariable or/and a nodeValue seeks boolean type of answer
             *   
             */

            string nodeVariableName = node.GetVariableName();
            string nodeValueString = node.GetFactValue().GetValue().ToString();
            bool typeAlreadySet = TypeAlreadySet(node.GetFactValue());
            Dictionary<string, FactValue<T>> tempFactMap = this.nodeSet.GetFactMap();
            Dictionary<string, FactValue<T>> tempInputMap = this.nodeSet.GetInputMap();
            LineType nodeLineType = node.GetLineType();

            //ComparisonLine type node and type of the node's value is clearly defined 
            if (LineType.COMPARISON.Equals(nodeLineType))
            {
                FactValueType nodeRHSType = ((ComparisonLine<T>)node).GetRHS().GetFactValueType();
                if (!nodeRHSType.Equals(FactValueType.STRING))
                {
                    if (nodeRHSType.Equals(FactValueType.DEFI_STRING))
                    {
                        fvt = FactValueType.STRING;
                    }
                    else if (typeAlreadySet)
                    {
                        fvt = nodeRHSType;
                    }
                    factValueTypeMap.Add(((ComparisonLine<T>)node).GetLHS(), fvt);

                }
                else if (nodeRHSType.Equals(FactValueType.STRING))
                {
                    if (tempInputMap.ContainsKey(((ComparisonLine<T>)node).GetLHS()))
                    {
                        fvt = tempInputMap[((ComparisonLine<T>)node).GetLHS()].GetFactValueType();
                    }
                    else if (tempInputMap.ContainsKey(((ComparisonLine<T>)node).GetRHS().GetValue().ToString()))
                    {
                        fvt = tempInputMap[((ComparisonLine<T>)node).GetRHS().GetValue().ToString()].GetFactValueType();
                    }
                    else if (tempFactMap.ContainsKey(((ComparisonLine<T>)node).GetLHS()))
                    {
                        fvt = tempFactMap[((ComparisonLine<T>)node).GetLHS()].GetFactValueType();
                    }
                    else if (tempFactMap.ContainsKey(((ComparisonLine<T>)node).GetRHS().GetValue().ToString()))
                    {
                        fvt = tempFactMap[((ComparisonLine<T>)node).GetRHS().GetValue().ToString()].GetFactValueType();
                    }
                    factValueTypeMap.Add(((ComparisonLine<T>)node).GetLHS(), fvt);
                    factValueTypeMap.Add(((ComparisonLine<T>)node).GetRHS().GetValue().ToString(), fvt);
                }

            }
            //ComparisonLine type node and type of the node's value is not clearly defined and not defined in INPUT nor FIXED list
            //ValueConclusionLine type node and it is 'A-statement' line, and variableName is not defined neither INPUT nor FIXED 
            else if (LineType.VALUE_CONCLUSION.Equals(nodeLineType))
            {
                if (tempInputMap.ContainsKey(nodeVariableName))
                {
                    fvt = tempInputMap[nodeVariableName].GetFactValueType();
                }
                else if (ast.GetWorkingMemory().ContainsKey(nodeValueString))
                {
                    FactValue<T> tempFv = ast.GetWorkingMemory()[nodeValueString];
                    if (tempFv.GetFactValueType().Equals(FactValueType.LIST))
                    {
                        fvt = FactValueType.LIST;
                    }
                    else
                    {
                        fvt = tempFv.GetFactValueType();
                    }
                }
                else
                {
                    fvt = FactValueType.BOOLEAN;

                }
                factValueTypeMap.Add(nodeVariableName, fvt);

            }

            return factValueTypeMap;
        }

        public bool TypeAlreadySet(FactValue<T> value)
        {
            bool hasAlreadySetType = false;

            FactValueType factValueType = value.GetFactValueType();
            if (factValueType.Equals(FactValueType.DEFI_STRING) || factValueType.Equals(FactValueType.INTEGER) || factValueType.Equals(FactValueType.DOUBLE)
                || factValueType.Equals(FactValueType.DATE) || factValueType.Equals(FactValueType.BOOLEAN) || factValueType.Equals(FactValueType.UUID)
            || factValueType.Equals(FactValueType.URL) || factValueType.Equals(FactValueType.HASH))
            {
                hasAlreadySetType = true;
            }
            return hasAlreadySetType;
        }

        public bool IsIterateLineChild(int nodeId)
        {
            bool isIterateLineChild = false;
            List<int> tempList = new List<int>();
            List< Node<T>> iterateLineList = nodeSet.GetNodeMap().Values.Where((node) => node.GetLineType().Equals(LineType.ITERATE)).ToList();

            iterateLineList.ForEach((iNode) => {
                List<int> iterateChildNodeList = this.nodeSet.GetDependencyMatrix().GetToChildDependencyList(iNode.GetNodeId());
                if (iterateChildNodeList.Contains(nodeId))
                {
                    tempList.Add(1);
                }
                else
                {
                    IsIterateLineChildAux(tempList, iterateChildNodeList, nodeId);
                }
            });

            if(tempList.Count > 0)
            {
                isIterateLineChild = true;
            }
            else
            {
                if (this.GetAssessmentState().GetMandatoryList().Contains(this.nodeSet.GetNodeIdMap()[nodeId]))
                {
                    this.GetAssessmentState().GetMandatoryList().Remove(this.nodeSet.GetNodeIdMap()[nodeId]);
                }
            }

            return isIterateLineChild;
        }

        public void IsIterateLineChildAux(List<int> tempList, List<int> iterateChildNodeList, int nodeId)
        {
            iterateChildNodeList.ForEach((id)=> {
                List<int> iterateChildNodeListAux = this.nodeSet.GetDependencyMatrix().GetToChildDependencyList(id);
                if (iterateChildNodeList.Contains(nodeId))
                {
                    tempList.Add(1);
                }
                else
                {
                    IsIterateLineChildAux(tempList, iterateChildNodeListAux, nodeId);
                }
            });
        }

        /*
         * this is to check whether or not a node can be evaluated with all information in the workingMemory. If there is information for a value of node's value(FactValue) or variableName, then the node can be evaluated otherwise not.
         * In order to do it, AssessmentState.workingMemory must contain a value for variable of the rule, 
         * and rule type must be either COMPARISON, ITERATE or VALUE_CONCLUSION because they are the ones only can be the most child nodes, 
         * and other type of node must be a parent of other types of node.  
         */
        public bool CanEvaluate(Node<T> node)
        {

            bool canEvaluate = false;
            LineType lineType = node.GetLineType();

            if (lineType.Equals(LineType.VALUE_CONCLUSION))
            {
                if (((ValueConclusionLine<T>)node).GetIsPlainStatementFormat() && ast.GetWorkingMemory().ContainsKey(node.GetVariableName()))
                {
                    /*
                     * If the node is in plain statement format then varibaleName has same value as nodeName,
                     * and if a value for either variableName or nodeName of the node is in workingMemory then it means the node has already been evaluated.
                     * Hence, 'canEvaluate' needs to be 'true' in this case.
                     */
                    canEvaluate = true;
                }
                else if (node.GetTokens().tokensList.Any((s) => s.Equals("IS IN LIST:"))
                        && ast.GetWorkingMemory().ContainsKey(node.GetFactValue().GetValue().ToString())
                        && ast.GetWorkingMemory().ContainsKey(node.GetVariableName()))
                {
                    canEvaluate = true;
                    FactValue<T> fv = node.SelfEvaluate(ast.GetWorkingMemory(), this.scriptEngine);

                    /*
                     * the reason why ast.setFact() is used here rather than this.feedAndwerToNode() is that LineType is already known, and target node object is already found. 
                     * node.selfEvaluation() returns a value of the node's self-evaluation hence, node.getNodeName() is used to store a value for the node itself into a workingMemory
                     */
                    ast.SetFact(node.GetNodeName(), fv);
                }
            }
            else if (lineType.Equals(LineType.COMPARISON))
            {
                FactValue<T> nodeRhsValue = ((ComparisonLine<T>)node).GetRHS();
                if (!nodeRhsValue.GetFactValueType().Equals(FactValueType.STRING)
                        && ast.GetWorkingMemory().ContainsKey(((ComparisonLine<T>)node).GetLHS()))
                {
                    canEvaluate = true;
                    if (!ast.GetWorkingMemory().ContainsKey(node.GetNodeName()))
                    {
                        ast.SetFact(node.GetNodeName(), node.SelfEvaluate(ast.GetWorkingMemory(), this.scriptEngine));
                    }
                }
                else if (nodeRhsValue.GetFactValueType().Equals(FactValueType.STRING)
                        && ast.GetWorkingMemory().ContainsKey(((ComparisonLine<T>)node).GetLHS())
                            && ast.GetWorkingMemory().ContainsKey(((ComparisonLine<T>)node).GetRHS().GetValue().ToString()))
                {
                    canEvaluate = true;
                    if (!ast.GetWorkingMemory().ContainsKey(node.GetNodeName()))
                    {
                        ast.SetFact(node.GetNodeName(), node.SelfEvaluate(ast.GetWorkingMemory(), this.scriptEngine));
                    }
                }
            }

            return canEvaluate;
        }


        /* 
         * this method is to add fact or set a node as a fact by using AssessmentState.setFact() method. it also is used to feed an answer to a being asked node.
         * once a fact is added then forward-chain is used to update all effected nodes' state, and workingMemory in AssessmentState class will be updated accordingly
         * the reason for taking nodeName instead nodeVariableName is that it will be easier to find an exact node with nodeName
         * rather than nodeVariableName because a certain nodeVariableName could be found in several nodes
         */
        public void FeedAnswerToNode(Node<T> targetNode, string questionName, T nodeValue, FactValueType nodeValueType, Assessment<T> ass)
        {
            FactValue<T> fv = null;

            if (nodeValueType.Equals(FactValueType.BOOLEAN))
            {
                fv = FactValue<T>.Parse(Boolean.Parse(nodeValue.ToString()));
            }
            else if (nodeValueType.Equals(FactValueType.DATE))
            {
                /*
                 * the string of nodeValue date format is dd/MM/YYYY
                 */
                string[] dateArray = Regex.Split(nodeValue.ToString(), "/");
                DateTime factValueInDate = new DateTime(Int32.Parse(dateArray[2]), Int32.Parse(dateArray[1]), Int32.Parse(dateArray[0]));
                

                fv = FactValue<T>.Parse(factValueInDate);
            }
            else if (nodeValueType.Equals(FactValueType.DOUBLE))
            {
                fv = FactValue<T>.Parse(Double.Parse(nodeValue.ToString()));
            }
            else if (nodeValueType.Equals(FactValueType.INTEGER))
            {
                fv = FactValue<T>.Parse(Int32.Parse(nodeValue.ToString()));
            }
            else if (nodeValueType.Equals(FactValueType.LIST))
            {
                fv = FactValue<T>.ParseList(nodeValue);
            }
            else if (nodeValueType.Equals(FactValueType.STRING))
            {
                fv = FactValue<T>.Parse(nodeValue.ToString());
            }
            else if (nodeValueType.Equals(FactValueType.DEFI_STRING))
            {
                fv = FactValue<T>.ParseDefiString(nodeValue.ToString());
            }
            else if (nodeValueType.Equals(FactValueType.HASH))
            {
                fv = FactValue<T>.ParseHash(nodeValue.ToString());
            }
            else if (nodeValueType.Equals(FactValueType.URL))
            {
                fv = FactValue<T>.ParseURL(nodeValue.ToString());
            }
            else if (nodeValueType.Equals(FactValueType.UUID))
            {
                fv = FactValue<T>.ParseUUID(nodeValue.ToString());
            }

            if (fv != null && !ass.GetNodeToBeAsked().GetLineType().Equals(LineType.ITERATE))
            {
                ast.SetFact(questionName, fv);
                ast.AddItemToSummaryList(questionName);// add currentRule into SummeryList as the rule determined


                if (targetNode.GetLineType().Equals(LineType.VALUE_CONCLUSION) && !((ValueConclusionLine<T>)targetNode).GetIsPlainStatementFormat())
                {
                    FactValue<T> selfEvalFactValue = targetNode.SelfEvaluate(ast.GetWorkingMemory(), this.scriptEngine);
                    ast.SetFact(targetNode.GetNodeName(), selfEvalFactValue); // add the value of targetNode itself into the workingMemory  
                    ast.AddItemToSummaryList(targetNode.GetNodeName());// add currentRule into SummeryList as the rule determined
                }
                else if (targetNode.GetLineType().Equals(LineType.COMPARISON))
                {
                    FactValue<T> rhsValue = ((ComparisonLine<T>)targetNode).GetRHS();
                    if((rhsValue.GetFactValueType().Equals(FactValueType.STRING)
                            && ast.GetWorkingMemory().ContainsKey(rhsValue.GetValue().ToString()))
                                || !rhsValue.GetFactValueType().Equals(FactValueType.STRING)
                      )
                    {
                        FactValue<T> selfEvalFactValue = targetNode.SelfEvaluate(ast.GetWorkingMemory(), this.scriptEngine);
                        ast.SetFact(targetNode.GetNodeName(), selfEvalFactValue); // add the value of targetNode itself into the workingMemory  
                        ast.AddItemToSummaryList(targetNode.GetNodeName());// add currentRule into SummeryList as the rule determined
                    }

                }


                /*
                 * once any rules are set as fact and stored into the workingMemory, back-propagation(forward-chaining) needs to be done
                 */
                BackPropagating(nodeSet.FindNodeIndex(targetNode.GetNodeName()));

            }
            else if (ass.GetNodeToBeAsked().GetLineType().Equals(LineType.ITERATE))
            {
                ((IterateLine<T>)ass.GetNodeToBeAsked()).IterateFeedAnswers(targetNode, questionName, nodeValue, nodeValueType, this.nodeSet, ast, ass);
                if (((IterateLine<T>)ass.GetNodeToBeAsked()).CanBeSelfEvaluated(ast.GetWorkingMemory()))
                {
                    BackPropagating(nodeSet.FindNodeIndex(ass.GetNodeToBeAsked().GetNodeName()));
                }
            }
        }


        public void BackPropagating(int nodeIndex)
        {
            List<Node<T>> nodeSortedList = nodeSet.GetNodeSortedList();
            int sortedListSize = nodeSortedList.Count;
            Enumerable.Range(0, sortedListSize).ToList().ForEach((i) =>
            {
                Node<T> tempNode = nodeSortedList[sortedListSize - (i + 1)];
                LineType lineType = tempNode.GetLineType();

                int tempNodeId = tempNode.GetNodeId();
                List<int> parentDependencyList = nodeSet.GetDependencyMatrix().GetFromParentDependencyList(tempNodeId);
                if (parentDependencyList.Count != 0)
                {
                    parentDependencyList.ForEach((parentId) =>
                    {
                        if ((nodeSet.GetDependencyMatrix().GetDependencyType(parentId, tempNodeId) & DependencyType.GetMandatory()) == DependencyType.GetMandatory()
                            && !ast.IsInclusiveList(tempNode.GetNodeName())
                           && !IsIterateLineChild(tempNode.GetNodeId()))
                        {
                            ast.AddItemToMandatoryList(tempNode.GetNodeName());
                        }

                        if (nodeIndex < (sortedListSize - (i + 1))) //case of all nodes located after the nodeIndex
                        {
                            if (HasChildren(tempNode.GetNodeId()))
                            {
                                if(!ast.GetWorkingMemory().ContainsKey(tempNode.GetNodeName())
                                    && CanDetermine(tempNode, lineType))
                                {
                                    if(!lineType.Equals(LineType.EXPR_CONCLUSION))
                                    {
                                        ast.AddItemToSummaryList(tempNode.GetNodeName());// add currentRule into SummeryList as the rule determined
                                    }
                                }
                            }
                            else
                            {
                                /*
                                 * ValueConclusionLine in 'A-statement' format does not need to be considered here due to the reason that
                                 * the case should be in the workingMemory if it is already asked.
                                 */
                                if (lineType.Equals(LineType.VALUE_CONCLUSION)
                                    && !((ValueConclusionLine<T>)tempNode).GetIsPlainStatementFormat()
                                    && ast.GetWorkingMemory().ContainsKey(((ValueConclusionLine<T>)tempNode).GetVariableName()))
                                {
                                    FactValue<T> fv = tempNode.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine);

                                    ast.SetFact(tempNode.GetNodeName(), fv);
                                    ast.AddItemToSummaryList(tempNode.GetNodeName());// add currentRule into SummeryList as the rule determined
                                }
                                else if (lineType.Equals(LineType.COMPARISON)
                                        && ast.GetWorkingMemory().ContainsKey(((ComparisonLine<T>)tempNode).GetLHS())
                                        && ast.GetWorkingMemory().ContainsKey(((ComparisonLine<T>)tempNode).GetRHS().GetValue().ToString()))
                                {
                                    FactValue<T> fv = tempNode.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine);

                                    ast.SetFact(tempNode.GetNodeName(), fv);
                                    ast.AddItemToSummaryList(tempNode.GetNodeName());// add currentRule into SummeryList as the rule determined
                                }

                            }

                        }
                        else // case of all nodes located before the nodeIndex
                        {
                            /*
                             * it the tempNode is located before the nodeIndex then there is need to check whether or not the tempNode is in the inclusiveList due to the reason that
                             * evaluating only relevant node could speed the propagation faster. In addition only relevant nodes can be traced by checking the inclusiveList.
                             */

                            if (ast.GetInclusiveList().Contains(tempNode.GetNodeName()))
                            {
                                /*
                                 * once a user feeds an answer to the engine, the engine will propagate the entire NodeSet or Assessment base on the answer
                                 * during the back-propagation, the engine checks if current node;
                                 * 1. has been determined;
                                 * 2. has any child nodes;
                                 * 3. can be determined with given facts in the workingMemory.
                                 * 
                                 * once the current checking node meets the condition then add it to the summaryList for summary view.
                                 */
                                if (!ast.GetWorkingMemory().ContainsKey(tempNode.GetNodeName())
                                        && HasChildren(tempNode.GetNodeId())
                                        && CanDetermine(tempNode, lineType)
                                   )
                                {
                                    if (!lineType.Equals(LineType.EXPR_CONCLUSION))
                                    {
                                        ast.AddItemToSummaryList(tempNode.GetNodeName()); // add currentRule into SummeryList as the rule determined
                                    }

                                }
                            }
                        }

                    });
                }
            });
        }


        /*
         *this method is to find all parent rules of a given rule, and add them into the ' inclusiveList' for future reference
         */
        public void AddParentIntoInclusiveList(Node<T> node)
        {
            List<int> nodeInDependencyList = nodeSet.GetDependencyMatrix().GetFromParentDependencyList(node.GetNodeId());
            if (nodeInDependencyList.Count != 0) // if rule has parents
            {
                nodeInDependencyList.ForEach((i)=> {
                    Node<T> parentNode = nodeSet.GetNodeMap()[nodeSet.GetNodeIdMap()[i]];
                    if (!ast.GetInclusiveList().Contains(parentNode.GetNodeName()))
                    {
                        ast.GetInclusiveList().Add(parentNode.GetNodeName());
                    }
                });

            }
        }

        public bool HasAllMandatoryChildAnswered(int nodeId)
        {

            List<int> mandatoryChildDependencyList = nodeSet.GetDependencyMatrix().GetMandatoryToChildDependencyList(nodeId);
            bool hasAllMandatoryChildAnswered = false;
            if (mandatoryChildDependencyList.Count != 0)
            {
                hasAllMandatoryChildAnswered = mandatoryChildDependencyList.All((childId)=>(ast.GetWorkingMemory().ContainsKey(nodeSet.GetNodeIdMap()[childId])
                                                                                                 && HasAllMandatoryChildAnswered(childId)));
            }
            else if(mandatoryChildDependencyList.Count == 0)
            {
                hasAllMandatoryChildAnswered = true;
            }

            return hasAllMandatoryChildAnswered;
        }
                                                              
        public bool CanDetermine(Node<T> node, LineType lineType)
        {
            bool canDetermine = false;
            /*
             * Any type of node/line can have either 'OR' or 'AND' type of child nodes
             *  
             * -----ValueConclusion Type
             * there will be two cases for this type
             *    V.1 the format of node is 'A -statement' so that 'TRUE' or "FALSE' value outcome case
             *         V.1.1 if it has 'OR' child nodes
             *               V.1.1.1 TRUE case
             *                       if there is any of child node is 'true'
             *                       then trim off 'UNDETERMINED' child nodes, which are not in 'workingMemory', other than 'MANDATORY' child nodes
             *               V.1.1.2 FALSE case
             *                       if its all 'OR' child nodes are determined and all of them are 'false'
             *         V.1.2 if it has 'AND' child nodes
             *               V.1.2.1 TRUE case
             *                       if its all 'AND' child nodes are determined and all of them are 'true'
             *               V.1.2.2 FALSE case
             *                       if its all 'AND' child nodes are determined and all of them are 'false'
             *                       , and there is no need to trim off 'UNDETERMINED' child nodes other than 'MANDATORY' child nodes
             *                       because since 'virtual node' is introduced, any parent nodes won't have 'OR' and 'AND' dependency at the same time
             *              
             *         V.1.3 other than above scenario it can't be determined in 'V.1' case
             *    
             *    V.2 a case of that the value in the node text can be used as a value of its node's variable (e.g. A IS B, B can be used as a value for variable, 'A' in this case if all its child nodes or one of its child node is true)
             *         V.2.1 if it has 'OR' child nodes
             *               V.2.1.1 the value CAN BE USED case
             *                       if its any of child node is 'true'
             *                       then trim off 'UNDETERMINED' child nodes, which are not in 'workingMemory', other than 'MANDATORY' child nodes
             *               V.2.1.2 the value CANNOT BE USED case
             *                       if its all 'OR' child nodes are determined and all of them are 'false'
             *         V.2.2 if it has 'AND' child nodes
             *               V.2.2.1 the value CAN BE USED case
             *                       if its all 'AND' child nodes are determined and all of them are 'true'
             *               V.2.2.2 the value CANNOT BE USED case
             *                       if its all 'AND' child nodes are determined and all of them are 'false'
             *                       , and there is no need to trim off 'UNDETERMINED' child nodes other than 'MANDATORY' child nodes
             *                       because since 'virtual node' is introduced, any parent nodes won't have 'OR' and 'AND' dependency at the same time
             *         
             *         V.2.3 other than above scenario it can't be determined in 'V.2' case
             *              
             *    
             * Note: the reason why only ResultType and ExpressionType are evaluated with selfEvaluation() is as follows;
             *       1. ComparisonType is only evaluated by comparing a value of rule's variable in workingMemory with the value in the node
             *       2. ExpressionType is only evaluated by retrieving a value(s) of needed child node(s)   
             *       3. ValueConclusionType is evaluated under same combination of various condition, and trimming dependency is involved.           *        
             *       
             */
            List<int> orToChildDependencies = nodeSet.GetDependencyMatrix().GetOrToChildDependencyList(node.GetNodeId());
            List<int> andToChildDependencies = nodeSet.GetDependencyMatrix().GetAndToChildDependencyList(node.GetNodeId());



            if (LineType.VALUE_CONCLUSION.Equals(lineType))
            {
                if (node.GetNodeName().Contains("IS IN LIST")
                        && ast.GetWorkingMemory().ContainsKey(node.GetVariableName())
                            && ast.GetWorkingMemory().ContainsKey(node.GetFactValue().GetValue().ToString()))
                {
                    ast.SetFact(node.GetNodeName(), node.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine));
                    canDetermine = true;
                }
                else
                {
                    bool isPlainStatementFormat = ((ValueConclusionLine<T>)node).GetIsPlainStatementFormat();
                    string nodeFactValueInString = node.GetFactValue().GetValue().ToString();
                    /*
                     * isAnyOrDependencyTrue() method contains trimming off method to cut off any 'UNDETERMINED' state 'OR' child nodes. 
                     */
                    if (andToChildDependencies.Count == 0 && orToChildDependencies.Count != 0) // rule has only 'OR' child rules 
                    {

                        if (IsAnyOrDependencyTrue(node, orToChildDependencies)) //TRUE case
                        {
                            int nodeId = node.GetNodeId();
                            if (nodeSet.GetDependencyMatrix().HasMandatoryChildNode(nodeId) && !HasAllMandatoryChildAnswered(nodeId))
                            {
                                return canDetermine;
                            }
                            canDetermine = true;

                            HandleValuConclusionLineTrueCase(node, isPlainStatementFormat, nodeFactValueInString);

                        }
                        else if (IsAllRelevantChildDependencyDetermined(node, orToChildDependencies) && !IsAnyOrDependencyTrue(node, orToChildDependencies)) //FALSE case
                        {
                            canDetermine = true;

                            HandleValueConclusionLineFalseCase(node, isPlainStatementFormat, nodeFactValueInString);

                        }
                    }
                    else if (andToChildDependencies.Count != 0 && orToChildDependencies.Count == 0)// node has only 'AND' child nodes
                    {
                        if (IsAllRelevantChildDependencyDetermined(node, andToChildDependencies) && IsAllAndDependencyTrue(node, andToChildDependencies)) // TRUE case
                        {
                            canDetermine = true;

                            HandleValuConclusionLineTrueCase(node, isPlainStatementFormat, nodeFactValueInString);

                        }
                        /*
                         * 'isAnyAndDependencyFalse()' contains a trimming off dependency method 
                         * due to the fact that all undetermined 'AND' child nodes need to be trimmed off when any 'AND' node is evaluated as 'NO'
                     * , which does not influence on determining a parent rule's evaluation.
                     * 
                         */
                        else if (IsAnyAndDependencyFalse(node, andToChildDependencies)) //FALSE case
                        {
                            int nodeId = node.GetNodeId();
                            if (nodeSet.GetDependencyMatrix().HasMandatoryChildNode(nodeId))
                            {
                                if (!HasAllMandatoryChildAnswered(nodeId))
                                {
                                    return canDetermine;
                                }
                            }
                            canDetermine = true;

                            HandleValueConclusionLineFalseCase(node, isPlainStatementFormat, nodeFactValueInString);
                        }
                    }
                }
            }
            else if (LineType.COMPARISON.Equals(lineType))
            {
                if (andToChildDependencies.Count == 0 && orToChildDependencies.Count != 0) // rule has only 'OR' child rules 
                {
                    /*
                     * the node might have a 'MANDATORY OR' child nodes so that the mandatory child nodes need being handled
                     */
                    if (HasAnyOrChildEvaluated(node.GetNodeId(), orToChildDependencies))
                    {
                        if (!HasAllMandatoryChildAnswered(node.GetNodeId()))
                        {
                            return canDetermine = false;
                        }
                        canDetermine = true;
                        ast.SetFact(node.GetNodeName(), node.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine));
                        ast.AddItemToSummaryList(node.GetNodeName());
                    }
                }
                else if (andToChildDependencies.Count != 0 && orToChildDependencies.Count == 0)// node has only 'AND' child nodes
                {
                    /*
                     * in this case they are all 'MANDATORY' child nodes
                     */
                    if (HasAllAndChildEvaluated(andToChildDependencies))
                    {
                        if (!HasAllMandatoryChildAnswered(node.GetNodeId()))
                        {
                            return canDetermine = false;
                        }
                        canDetermine = true;
                        ast.SetFact(node.GetNodeName(), node.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine));
                        ast.AddItemToSummaryList(node.GetNodeName());
                    }
                }

            }
            else if (LineType.EXPR_CONCLUSION.Equals(lineType))
            {
                if (andToChildDependencies.Count == 0 && orToChildDependencies.Count != 0) // rule has only 'OR' child rules 
                {
                    /*
                     * the node might have a 'MANDATORY OR' child nodes so that the mandatory child nodes need being handled
                     */
                    if (HasAnyOrChildEvaluated(node.GetNodeId(), orToChildDependencies))
                    {
                        if (!HasAllMandatoryChildAnswered(node.GetNodeId()))
                        {
                            return canDetermine = false;
                        }
                        canDetermine = true;
                        ast.SetFact(node.GetVariableName(), node.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine));
                        ast.SetFact(node.GetNodeName(), node.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine)); //inserting same value for node's name is for the purpose of display equation

                        ast.AddItemToSummaryList(node.GetVariableName());
                        ast.AddItemToSummaryList(node.GetNodeName()); // inserting node's name is to find its evaluated value from the workingMemory with its name
                    }
                }
                else if (andToChildDependencies.Count != 0 && orToChildDependencies.Count == 0)// node has only 'AND' child nodes
                {
                    /*
                     * in this case they are all 'MANDATORY' child nodes
                     */
                    if (HasAllAndChildEvaluated(andToChildDependencies))
                    {
                        if (!HasAllMandatoryChildAnswered(node.GetNodeId()))
                        {
                            return canDetermine = false;
                        }
                        canDetermine = true;
                        ast.SetFact(node.GetVariableName(), node.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine));
                        ast.SetFact(node.GetNodeName(), node.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine)); //inserting same value for node's name is for the purpose of display equation

                        ast.AddItemToSummaryList(node.GetVariableName());
                        ast.AddItemToSummaryList(node.GetNodeName()); // inserting node's name is to find its evaluated value from the workingMemory with its name
                    }
                }
                else
                {
                    if (HasAnyOrChildEvaluated(node.GetNodeId(), orToChildDependencies) && HasAllAndChildEvaluated(andToChildDependencies))
                    {
                        if (!HasAllMandatoryChildAnswered(node.GetNodeId()))
                        {
                            return canDetermine = false;
                        }
                        canDetermine = true;
                        ast.SetFact(node.GetVariableName(), node.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine));
                        ast.SetFact(node.GetNodeName(), node.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine)); //inserting same value for node's name is for the purpose of display equation

                        ast.AddItemToSummaryList(node.GetVariableName());
                        ast.AddItemToSummaryList(node.GetNodeName()); // inserting node's name is to find its evaluated value from the workingMemory with its name
                    }
                }
            }
            else if (LineType.ITERATE.Equals(lineType))
            {
                if (((IterateLine<T>)node).CanBeSelfEvaluated(ast.GetWorkingMemory()))
                {
                    ast.SetFact(node.GetNodeName(), node.SelfEvaluate(ast.GetWorkingMemory(), scriptEngine));
                    ast.AddItemToSummaryList(node.GetVariableName());
                }
            }

            return canDetermine;
        }


        private bool HasAnyOrChildEvaluated(int parentNodeId, List<int> orToChildDependencies)
        {

            bool hasAnyOrChildEvaluated = orToChildDependencies.Any((i) => (ast.GetWorkingMemory().ContainsKey(nodeSet.GetNodeByNodeId(i).GetVariableName())
                                                                                    && (nodeSet.GetDependencyMatrix().GetDependencyType(parentNodeId, i) & DependencyType.GetMandatory()) == DependencyType.GetMandatory()
                                                                            )
                                                                            ||
                                                                            ast.GetWorkingMemory().ContainsKey(nodeSet.GetNodeByNodeId(i).GetVariableName())
                                                                       );

            return hasAnyOrChildEvaluated;
        }

        private bool HasAllAndChildEvaluated(List<int> andToChildDependencies)
        {
                        bool hasAllAndChildEvaluated = andToChildDependencies.All((i) => ast.GetWorkingMemory().ContainsKey(nodeSet.GetNodeByNodeId(i).GetVariableName()));

            return hasAllAndChildEvaluated;
        }

        private void HandleValuConclusionLineTrueCase(Node<T> node, bool isPlainStatementFormat, string nodeFactValueInString)
        {
            ast.SetFact(node.GetNodeName(), FactValue<T>.Parse(true));

            if (!isPlainStatementFormat)
            {
                if (ast.GetWorkingMemory().ContainsKey(nodeFactValueInString))
                {
                    ast.SetFact(node.GetVariableName(), ast.GetWorkingMemory()[nodeFactValueInString]);
                }
                else
                {
                    ast.SetFact(node.GetVariableName(), node.GetFactValue());
                }
                ast.AddItemToSummaryList(node.GetVariableName());
            }
        }
        private void HandleValueConclusionLineFalseCase(Node<T> node, bool isPlainStatementFormat, string nodeFactValueInString)
        {
            ast.SetFact(node.GetNodeName(), FactValue<T>.Parse(false));

            if (!isPlainStatementFormat)
            {
                if (ast.GetWorkingMemory().ContainsKey(nodeFactValueInString))
                {
                    ast.SetFact(node.GetVariableName(), FactValue<T>.Parse("NOT " + ast.GetWorkingMemory()[nodeFactValueInString]));
                }
                else
                {
                    ast.SetFact(node.GetVariableName(), FactValue<T>.Parse("NOT " + nodeFactValueInString));
                }
                ast.AddItemToSummaryList(node.GetVariableName());
            }
        }

        public string GetDefaultGoalRuleQuestion()
        {
            return nodeSet.GetDefaultGoalNode().GetNodeName();
        }
        public string GetAssessmentGoalRuleQuestion(Assessment<T> ass)
        {
            return ass.GetGoalNode().GetNodeName();
        }

        public FactValue<T> GetDefaultGoalRuleAnswer()
        {
            return ast.GetWorkingMemory()[nodeSet.GetDefaultGoalNode().GetVariableName()];
        }

        public FactValue<T> GetAssessmentGoalRuleAnswer(Assessment<T> ass)
        {
            return ast.GetWorkingMemory()[ass.GetGoalNode().GetVariableName()];
        }


        /*
         Returns boolean value that can determine whether or not the given rule has any children
         this method is used within the process of backward chaining.
         */
        public bool HasChildren(int nodeId)
        {
            bool hasChildren = false;
            if (nodeSet.GetDependencyMatrix().GetToChildDependencyList(nodeId).Count != 0)
            {
                hasChildren = true;
                nodeSet.GetDependencyMatrix().GetToChildDependencyList(nodeId).ForEach((item) =>{
                    Node<T> nodeName = nodeSet.GetNodeByNodeId(item);
                    AddChildRuleIntoInclusiveList(nodeName);
                });
            }
            return hasChildren;
        }

        /*
        the method adds all children rules of relevant parent rule into the 'inlcusiveList' if they are not in the list.
        */
        public void AddChildRuleIntoInclusiveList(Node<T> node)
        {

            List<int> childrenListOfNode = nodeSet.GetDependencyMatrix().GetToChildDependencyList(node.GetNodeId());
            childrenListOfNode.ForEach((item) => {
                string childNodeName = nodeSet.GetNodeMap()[nodeSet.GetNodeIdMap()[item]].GetNodeName();
                if (!ast.GetInclusiveList().Contains(childNodeName) && !ast.GetExclusiveList().Contains(childNodeName))
                {
                    ast.GetInclusiveList().Add(childNodeName);
                }
            });

        }


        public bool IsAnyOrDependencyTrue(Node<T> node, List<int> orChildDependencies)
        {
            bool isAnyOrDependencyTrue = false;
            targetNode = node;
            if (orChildDependencies.Count != 0)
            {

                List<int> trueOrChildList = new List<int>();
                orChildDependencies.ForEach((item)=> {
                    if (ast.IsInclusiveList(nodeSet.GetNodeIdMap()[item])
                        && ast.GetWorkingMemory().ContainsKey(nodeSet.GetNodeIdMap()[item]))
                    {
                        if ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetKnown()) == DependencyType.GetKnown()
                          && ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetNot()) != DependencyType.GetNot())
                      )
                        {
                            trueOrChildList.Add(item);
                            if (!ast.GetWorkingMemory().ContainsKey("KNOWN " + nodeSet.GetNodeIdMap()[item]))
                            {
                                ast.SetFact("KNOWN " + nodeSet.GetNodeIdMap()[item], FactBooleanValue<T>.Parse(true));
                                ast.AddItemToSummaryList("KNOWN " + nodeSet.GetNodeIdMap()[item]);
                            }

                        }
                        else if (ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]].GetValue().Equals(true)
                                && ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetNot()) != DependencyType.GetNot()))
                        {

                            trueOrChildList.Add(item);
                        }
                        else if (ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]].GetValue().Equals(false)
                                && ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetNot()) == DependencyType.GetNot()))
                        {
                            trueOrChildList.Add(item);

                            if (!ast.GetWorkingMemory().ContainsKey("NOT " + nodeSet.GetNodeIdMap()[item]))
                            {
                                ast.SetFact("NOT " + nodeSet.GetNodeIdMap()[item], FactBooleanValue<T>.Parse(true));
                                ast.AddItemToSummaryList("NOT " + nodeSet.GetNodeIdMap()[item]);
                            }

                        }
                    }
                });


                if (trueOrChildList.Count != 0)
                {
                    isAnyOrDependencyTrue = true;
                    orChildDependencies.ForEach((i) => {
                        trueOrChildList.ForEach((n) => {
                            if (i != n)
                            {
                                TrimDependency(node, i);
                            }
                        });
                    });
                }
            }
            return isAnyOrDependencyTrue;
        }

        public void TrimDependency(Node<T> parentNode, int childNodeId)
        {
            int parentNodeId = parentNode.GetNodeId();
            int dpType = nodeSet.GetDependencyMatrix().GetDependencyMatrixArray()[parentNodeId, childNodeId];
            int mandatoryDependencyType = DependencyType.GetMandatory();
            List<int> parentDependencyList = nodeSet.GetDependencyMatrix().GetFromParentDependencyList(childNodeId);

            if ((parentDependencyList.Count > 1                                                                                            // the child has more than one parent,
                    && parentDependencyList.All((parent) => ast.GetWorkingMemory().ContainsKey(nodeSet.GetNodeIdMap()[parent]))
                        && !parentDependencyList.Any((parent) => (nodeSet.GetDependencyMatrix().GetDependencyMatrixArray()[parent, childNodeId]& mandatoryDependencyType) == mandatoryDependencyType)) //the child has no Mandatory dependency parents
                ||
                (parentDependencyList.Count == 1                                                                                           // the child has only one parent
                    && ((dpType & mandatoryDependencyType) != mandatoryDependencyType)))                                                    // the dependency is not 'MANDATORY'
            {
                ast.GetInclusiveList().Remove(nodeSet.GetNodeIdMap()[childNodeId]);
                if (!ast.GetExclusiveList().Contains(nodeSet.GetNodeIdMap()[childNodeId]))
                {
                    ast.GetExclusiveList().Add(nodeSet.GetNodeIdMap()[childNodeId]);
                }
                List<int> childDependencyListOfChildNode = nodeSet.GetDependencyMatrix().GetToChildDependencyList(childNodeId);
                if (childDependencyListOfChildNode.Count != 0)
                {
                    childDependencyListOfChildNode.ForEach((item) =>{
                        TrimDependency(nodeSet.GetNodeByNodeId(childNodeId), item);
                    });
                }
            }

        }

        public bool IsAnyAndDependencyFalse(Node<T> node, List<int> andChildDependencies)
        {
            bool isAnyAndDependencyFalse = false;
            targetNode = node;

            if (andChildDependencies.Count != 0)
            {
                List<int> falseAndList = new List<int>();

                andChildDependencies.ForEach((item) => {
                    if (ast.GetWorkingMemory().ContainsKey(nodeSet.GetNodeIdMap()[item]))
                    {
                        if (ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]].GetValue().Equals(false)
                                && ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetNot()) != DependencyType.GetNot())
                                && ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetKnown()) != DependencyType.GetKnown()))
                        {
                            falseAndList.Add(item);
                        }
                        else if (ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]].GetValue().Equals(true)
                                    && ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetNot()) == DependencyType.GetNot())
                                    && ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetKnown()) != DependencyType.GetKnown())
                        )
                        {
                            if (!ast.GetWorkingMemory().ContainsKey("NOT " + nodeSet.GetNodeIdMap()[item]))
                            {
                                ast.SetFact("NOT " + nodeSet.GetNodeIdMap()[item], FactBooleanValue<T>.Parse(false));
                                ast.AddItemToSummaryList("NOT " + nodeSet.GetNodeIdMap()[item]);
                            }

                            falseAndList.Add(item);
                        }
                        else if ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & (DependencyType.GetNot() | DependencyType.GetKnown())) == (DependencyType.GetNot() | DependencyType.GetKnown()))
                        {
                            if (!ast.GetWorkingMemory().ContainsKey("NOT KNOWN " + nodeSet.GetNodeIdMap()[item]))
                            {
                                ast.SetFact("NOT KNOWN " + nodeSet.GetNodeIdMap()[item], FactBooleanValue<T>.Parse(false));
                                ast.AddItemToSummaryList("NOT KNOWN " + nodeSet.GetNodeIdMap()[item]);
                            }

                            falseAndList.Add(item);
                        }
                    }
                });

                if (falseAndList.Count > 0)
                {
                    isAnyAndDependencyFalse = true;
                    andChildDependencies.ForEach((i) => {
                        falseAndList.ForEach((f) => {
                            if (i != f)
                            {
                                TrimDependency(node, i);
                            }
                        });
                    });
                }
                else if (andChildDependencies.Count == 0)
                {
                    isAnyAndDependencyFalse = true;
                }
            }

            return isAnyAndDependencyFalse;
        }


        public bool IsAllAndDependencyTrue(Node<T> node, List<int> andChildDependencies)
        {
            bool isAllAndTrue = false;
            targetNode = node;

            List<int> determinedTrueAndChildDependencies = new List<int>();
            andChildDependencies.ForEach((item) => {
                if (ast.IsInclusiveList(nodeSet.GetNodeIdMap()[item])
                && ast.GetWorkingMemory().ContainsKey(nodeSet.GetNodeIdMap()[item]))
                {
                    if (ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]].GetValue().Equals(true)
                    && ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetNot()) != DependencyType.GetNot()))
                    {
                        determinedTrueAndChildDependencies.Add(item);
                    }
                    else if ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetKnown()) == DependencyType.GetKnown()
                          && ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetNot()) != DependencyType.GetNot()))
                    {
                        if (!ast.GetWorkingMemory().ContainsKey("KNOWN " + nodeSet.GetNodeIdMap()[item]))
                        {
                            ast.SetFact("KNOWN " + nodeSet.GetNodeIdMap()[item], FactBooleanValue<T>.Parse(false));
                            ast.AddItemToSummaryList("KNOWN " + nodeSet.GetNodeIdMap()[item]);
                        }

                        determinedTrueAndChildDependencies.Add(item);
                    }
                    else if (ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]].GetValue().Equals(false)
                          && ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetNot()) == DependencyType.GetNot())
                          && ((nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetKnown()) != DependencyType.GetKnown()))
                    {
                        if (!ast.GetWorkingMemory().ContainsKey("NOT " + nodeSet.GetNodeIdMap()[item]))
                        {
                            ast.SetFact("NOT " + nodeSet.GetNodeIdMap()[item], FactBooleanValue<T>.Parse(false));
                            ast.AddItemToSummaryList("NOT " + nodeSet.GetNodeIdMap()[item]);
                        }

                        determinedTrueAndChildDependencies.Add(item);
                    }
                }

            });

            if (andChildDependencies != null && determinedTrueAndChildDependencies.Count == andChildDependencies.Count)
            {
                isAllAndTrue = true;
            }

            return isAllAndTrue;
        }

        public bool IsAllRelevantChildDependencyDetermined(Node<T> node, List<int> allChildDependencies)
        {
            bool isAllRelevantChildDependencyDetermined = false;
            targetNode = node;

            List<int> determinedAndOutDependencies = new List<int>();
            allChildDependencies.ForEach((item) =>
            {
                if (ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]] != null
                    && (nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & (DependencyType.GetNot() | DependencyType.GetKnown())) == (DependencyType.GetNot() | DependencyType.GetKnown())
                    && !ast.GetWorkingMemory().ContainsKey("NOT KNOWN " + nodeSet.GetNodeIdMap()[item]))
                {
                    ast.SetFact("NOT KNOWN " + nodeSet.GetNodeIdMap()[item], FactBooleanValue<T>.Parse(false));
                    ast.AddItemToSummaryList("NOT KNOWN " + nodeSet.GetNodeIdMap()[item]);
                }
                else if (ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]] != null
                         && ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]].GetValue().Equals(false)
                         && (nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetNot()) == DependencyType.GetNot()
                         && !ast.GetWorkingMemory().ContainsKey("NOT " + nodeSet.GetNodeIdMap()[item]))
                {
                    ast.SetFact("NOT " + nodeSet.GetNodeIdMap()[item], FactBooleanValue<T>.Parse(true));
                    ast.AddItemToSummaryList("NOT " + nodeSet.GetNodeIdMap()[item]);
                }
                else if (ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]] != null
                         && (nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetKnown()) == DependencyType.GetKnown()
                         && !ast.GetWorkingMemory().ContainsKey("KNOWN " + nodeSet.GetNodeIdMap()[item]))
                {
                    ast.SetFact("KNOWN " + nodeSet.GetNodeIdMap()[item], FactBooleanValue<T>.Parse(true));
                    ast.AddItemToSummaryList("KNOWN " + nodeSet.GetNodeIdMap()[item]);
                }
                else if (ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]] != null
                         && ast.GetWorkingMemory()[nodeSet.GetNodeIdMap()[item]].GetValue().Equals(true)
                         && (nodeSet.GetDependencyMatrix().GetDependencyType(targetNode.GetNodeId(), item) & DependencyType.GetNot()) == DependencyType.GetNot()
                            && !ast.GetWorkingMemory().ContainsKey("NOT " + nodeSet.GetNodeIdMap()[item]))
                {
                    ast.SetFact("NOT " + nodeSet.GetNodeIdMap()[item], FactBooleanValue<T>.Parse(false));
                    ast.AddItemToSummaryList("NOT " + nodeSet.GetNodeIdMap()[item]);
                }


                if (ast.IsInclusiveList(nodeSet.GetNodeIdMap()[item])
                && ast.GetWorkingMemory().ContainsKey(nodeSet.GetNodeIdMap()[item]))
                {
                    determinedAndOutDependencies.Add(item);
                }
            }
                                                               );


            if (allChildDependencies != null && determinedAndOutDependencies.Count == allChildDependencies.Count)
            {
                isAllRelevantChildDependencyDetermined = true;
            }


            return isAllRelevantChildDependencyDetermined;
        }


        public List<string> GenerateSortedSummaryList()
        {
            List<string> sortedSummaryList = new List<string>();
            nodeSet.GetNodeSortedList().ForEach((node) =>{
                if (ast.GetSummaryList().Contains(node.GetNodeName()))
                {
                    sortedSummaryList.Add(node.GetNodeName());
                }
                if (ast.GetSummaryList().Contains("NOT " + node.GetNodeName()))
                {
                    sortedSummaryList.Add("NOT " + node.GetNodeName());
                }
                if (ast.GetSummaryList().Contains("KNOWN " + node.GetNodeName()))
                {
                    sortedSummaryList.Add("KNOWN " + node.GetNodeName());
                }
                if (ast.GetSummaryList().Contains("NOT KNOWN " + node.GetNodeName()))
                {
                    sortedSummaryList.Add("NOT KNOWN " + node.GetNodeName());
                }
            });

            ast.GetSummaryList().ForEach((nodeName) =>{
                if (!sortedSummaryList.Contains(nodeName))
                {
                    sortedSummaryList.Insert(1, nodeName);
                }
            });

            return sortedSummaryList;
        }

        //    public boolean allNeedsChildDetermined(Node parentNode, List<Integer> outDependency)
        //    {
        //          boolean allNeedsChildDetermined = false;
        //          
        //          int mandatoryAndDependencyType = DependencyType.getMandatory() | DependencyType.getAnd();
        //          List<Integer> determinedList = outDependency.stream().filter(i -> (nodeSet.getDependencyMatrix().getDependencyMatrixArray()[parentNode.getNodeId()][i] & mandatoryAndDependencyType) == mandatoryAndDependencyType
        //                                              && ast.getWorkingMemory().containsKey(nodeSet.getNodeMap().get(nodeSet.getNodeIdMap().get(i)).getVariableName())).collect(Collectors.toList());
        //          
        //          if(outDependency.size() == determinedList.size())
        //          {
        //              allNeedsChildDetermined = true;
        //          }       
        //          
        //          return allNeedsChildDetermined;
        //    }







        /**
         * make a summary of the assessment rules and answers as a html document
         * using a template for the structure and replacing markers the values
         * @return html
         */
        //    public String generateAssessmentSummaryFromTemplate()
        //    {                 
        //      
        //      Map<String, String> map = new HashMap<>();
        //      
        //      
        //      int i = 0 ; 
        //      for (String ruleName : ast.getInclusiveList())
        //      {
        //          String ruleState = ast.getWorkingMemory().get(ruleName);
        //          if (ruleState != null) {
        //              map.put("rules["+i+"].number", Integer.toString(i+1));//add question to array
        //              map.put("rules["+i+"].question", ruleName);//add question to array
        //              map.put("rules["+i+"].answer", ruleState);// add answer to array
        //              i++;
        //          }
        //      }
        //      map.put("rules[]", Integer.toString(i)); //specify size of array
        //      String html = Document.getTemplate("assessment_summary.xml");
        //      html = Document.replaceValuesInHtml(html, map);
        //      
        //      return html;
        //  }
        //
        //    

        /*
        this method is to reset 'workingMemory' list and 'inclusiveList'
        usage of this method will depend on a user. if a user wants to continue to assessment on a same veteran with same conditions
        then don't need to reset 'workingMemory' and 'inclusiveList' otherwise reset them.
        */
        public void ResetWorkingMemoryAndInclusiveList()
        {
            if (ast.GetInclusiveList().Count != 0)
            {
                ast.GetInclusiveList().Clear();
            }
            if (ast.GetWorkingMemory().Count != 0)
            {
                ast.GetWorkingMemory().Clear();
            }
        }


        /*
         * this is to generate Assessment Summary
         */
        public JObject[] GenerateAssessmentSummary()
        {
            List<JObject> tempSummaryList = new List<JObject>();
            this.GetAssessmentState().GetSummaryList().ForEach((item) =>{
                JObject objectNode = new JObject();
                objectNode.Add("nodeText", item);
                objectNode.Add("nodeValue", this.GetAssessmentState().GetWorkingMemory()[item].GetValue().ToString());
                tempSummaryList.Add(objectNode);
            });

            return tempSummaryList.ToArray();
        }

        public void EditAnswer(string question)
        {

            List<string> tempSummaryList = this.GetAssessmentState().GetSummaryList();
            int indexOfQuestionToBeEdited = tempSummaryList.IndexOf(question);
            Dictionary<string, FactValue<T>> tempWorkingMemory = this.GetAssessmentState().GetWorkingMemory();
                    
            /*
             * following two lines are to reset 'exclusiveList' and 'inclusiveList' which are for tracking all relevant branches by cutting dependencies
             */
            this.GetAssessmentState().SetExclusiveList(new List<string>());
            this.GetAssessmentState().SetExclusiveList(new List<string>());
            
            tempWorkingMemory.Remove(question); //need to remove values of 'question' key from workingMemory because it needs editing
            
            /*
             * the reason of doing following lines is to re-establish 'inclusiveList' and 'exclusiveList'
             * which manage cutting all irrelevant branches within the rule tree based on fed answers.
             * all branches up to the point of 'to-be-edited-question' need re-establishment and other branches after the 'question'
             * don't need to be re-established because those may not irrelevant to effect decision at the end unless they are appeared
             * during the questionnaire after all re-establishment.
             */
            Enumerable.Range(0, tempSummaryList.Count).ToList().ForEach((index) =>{
                if(index < indexOfQuestionToBeEdited) {
                    Node<T> node = this.GetNextQuestion(this.GetAssessment());
                    if(ass.GetNodeToBeAsked().GetLineType().Equals(LineType.ITERATE))
                    {
                        ass.SetAuxNodeToBeAsked(node);
                    }
                    List<string> questionnaireFromNode = this.GetQuestionsFromNodeToBeAsked(node);
                    questionnaireFromNode.ForEach((questionItem) =>{
                        if(tempSummaryList.Contains(questionItem))
                        {
                            FactValue<T> fv = tempWorkingMemory[questionItem];
                            this.FeedAnswerToNode(node, questionItem, fv.GetValue(), fv.GetFactValueType(), ass);
                        }
                    });
                }
            });
        
    
        }

        /*
         * this is to find a condition with a list of given keyword
         */
        public List<string> FindCondition(string keyword)
        {
            int initialSize = nodeSet.GetNodeSortedList().Count;
            List<string> conditionList = new List<string>(initialSize);
            List<string> questionList = new List<string>(initialSize);
            foreach (Node<T> node in nodeSet.GetNodeSortedList())
            {
                if (nodeSet.GetDependencyMatrix().GetToChildDependencyList(node.GetNodeId()).Count == 0)
                {
                    questionList.Add(node.GetNodeName());
                }
            }

            string[] keywordArray = Regex.Split(keyword, "\\W"); // split the keyword by none word character including whitespace.
            int keywordArrayLength = keywordArray.Length;
            int numberOfMatched = 0;
            foreach (string ruleName in questionList)
            {
                numberOfMatched = 0;
                for (int i = 0; i < keywordArrayLength; i++)
                {
                    if (ruleName.Contains(keywordArray[i]))
                    {
                        numberOfMatched++;
                    }
                }
                if (numberOfMatched == keywordArrayLength)
                {
                    conditionList.Add(ruleName);
                }
            }

            return conditionList;
        }

    }
}
