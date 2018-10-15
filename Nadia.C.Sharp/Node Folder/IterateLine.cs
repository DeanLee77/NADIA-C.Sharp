using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.InferenceEngineFolder;
using Nadia.C.Sharp.RuleParserFolder;
using Newtonsoft.Json.Linq;

namespace Nadia.C.Sharp.NodeFolder
{
    public class IterateLine<T>: Node<T>
    {
        private String numberOfTarget;
        private NodeSet<T> iterateNodeSet;
        private String givenListName;
        private int givenListSize;
        private InferenceEngine<T> iterateIE;

        

        public IterateLine(string parentText, Tokens tokens): base(parentText, tokens)
        {
            givenListSize = 0;
            
        }

        public String GetGivenListName() {
            return this.givenListName;
        }
        
        public String GetNumberOfTarget() {
            return this.numberOfTarget;
        }
        public NodeSet<T> CreateIterateNodeSet(NodeSet<T> parentNodeSet)
        {

            DependencyMatrix parentDM = parentNodeSet.GetDependencyMatrix();
            Dictionary<string, Node<T>> parentNodeMap = parentNodeSet.GetNodeMap();
            Dictionary<int?, string> parentNodeIdMap = parentNodeSet.GetNodeIdMap();
            
            
            Dictionary<string, Node<T>> thisNodeMap = new Dictionary<string, Node<T>>();
            Dictionary<int?, string> thisNodeIdMap = new Dictionary<int?, string>();
            List<Dependency<T>> tempDependencyList = new List<Dependency<T>>();
            NodeSet<T> newNodeSet = new NodeSet<T>();

            thisNodeMap.Add(this.nodeName, this);
            thisNodeIdMap.Add(this.nodeId, this.nodeName);
            Enumerable.Range(1, this.givenListSize + 1).ToList().ForEach((nth) =>
            {
                parentDM.GetToChildDependencyList(this.nodeId).ForEach((item) =>
                {
                    if (this.GetNodeId() + 1 != item) // not first question id
                    {
                        Node<T> tempChildNode = parentNodeMap[parentNodeIdMap[item]];
                        LineType lineType = tempChildNode.GetLineType();

                        Node<T> tempNode = null;
                        string nextNThInString = Oridinal(nth);

                        if (lineType.Equals(LineType.VALUE_CONCLUSION))
                        {
                            tempNode = new ValueConclusionLine<T>(nextNThInString + " " + this.GetVariableName() + " " + tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                        }
                        else if (lineType.Equals(LineType.COMPARISON))
                        {
                            tempNode = new ComparisonLine<T>(nextNThInString + " " + this.GetVariableName() + " " + tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                            FactValue<T> tempNodeFv = ((ComparisonLine<T>)tempNode).GetRHS();
                            if (tempNodeFv.GetFactValueType().Equals(FactValueType.STRING))
                            {
                                FactValue<T> tempFv = FactValue<T>.Parse(nextNThInString + " " + this.GetVariableName() + " " + tempNodeFv);
                                tempNode.SetValue(tempFv);
                            }
                        }
                        else if (lineType.Equals(LineType.EXPR_CONCLUSION))
                        {
                            tempNode = new ExprConclusionLine<T>(nextNThInString + " " + this.GetVariableName() + " " + tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                        }


                        thisNodeMap.Add(tempNode.GetNodeName(), tempNode);
                        thisNodeIdMap.Add(tempNode.GetNodeId(), tempNode.GetNodeName());
                        tempDependencyList.Add(new Dependency<T>(this, tempNode, parentDM.GetDependencyType(this.nodeId, item)));

                        CreateIterateNodeSetAux(parentDM, parentNodeMap, parentNodeIdMap, thisNodeMap, thisNodeIdMap, tempDependencyList, item, tempNode.GetNodeId(), nextNThInString);

                    }
                    else // first question id
                    {
                        Node<T> firstIterateQuestionNode = parentNodeSet.GetNodeByNodeId(parentNodeSet.GetDependencyMatrix().GetToChildDependencyList(this.GetNodeId()).Min());
                        thisNodeMap.Add(firstIterateQuestionNode.GetNodeName(), firstIterateQuestionNode);
                        thisNodeIdMap.Add(item, firstIterateQuestionNode.GetNodeName());
                        tempDependencyList.Add(new Dependency<T>(this, firstIterateQuestionNode, parentDM.GetDependencyType(this.nodeId, item)));
                    }
                });
            });

           
            
            int numberOfRules = Node<T>.GetStaticNodeId();
            int[,] dependencyMatrix = new int[numberOfRules, numberOfRules];
        

            tempDependencyList.ForEach(dp => {
                int parentId = dp.GetParentNode().GetNodeId();
                int childId = dp.GetChildNode().GetNodeId();
                int dpType = dp.GetDependencyType();
                dependencyMatrix[parentId, childId] = dpType;
            });
            
            
            
            newNodeSet.SetNodeIdMap(thisNodeIdMap);
            newNodeSet.SetNodeMap(thisNodeMap);
            newNodeSet.SetDependencyMatrix(new DependencyMatrix(dependencyMatrix));
            newNodeSet.SetFactMap(parentNodeSet.GetFactMap());
            newNodeSet.SetNodeSortedList(TopoSort<T>.DfsTopoSort(thisNodeMap, thisNodeIdMap, dependencyMatrix));
    //      newNodeSet.getNodeSortedList().stream().forEachOrdered(item->System.out.println(item.getNodeId()+"="+item.getNodeName()));
            return newNodeSet;
        
        }
        
        public void CreateIterateNodeSetAux(DependencyMatrix parentDM, Dictionary<string, Node<T>> parentNodeMap, Dictionary<int?, string> parentNodeIdMap, Dictionary<string, Node<T>> thisNodeMap, Dictionary<int?, string> thisNodeIdMap, List<Dependency<T>> tempDependencyList, int originalParentId, int modifiedParentId, string nextNThInString)
        {
            List<int> childDependencyList = parentDM.GetToChildDependencyList(originalParentId);

            if(childDependencyList.Count > 0)
            {
                childDependencyList.ForEach((item) =>{
                    Node<T> tempChildNode = parentNodeMap[parentNodeIdMap[item]];
                    LineType lt = tempChildNode.GetLineType();
                    
                    Node<T> tempNode = thisNodeMap[nextNThInString+" "+this.GetVariableName()+" "+tempChildNode.GetNodeName()];
                    if(tempNode == null)
                    {
                        if(lt.Equals(LineType.VALUE_CONCLUSION))
                        {
                                tempNode = new ValueConclusionLine<T>(nextNThInString+" "+this.GetVariableName()+" "+tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                                
                                if(parentNodeMap[parentNodeIdMap[originalParentId]].GetLineType().Equals(LineType.EXPR_CONCLUSION))
                                {
                                    ExprConclusionLine<T> exprTempNode = thisNodeMap[thisNodeIdMap[modifiedParentId]] as ExprConclusionLine<T>;
                                string replacedString = exprTempNode.GetEquation().GetValue().ToString().Replace(tempChildNode.GetNodeName(), nextNThInString+" "+this.GetVariableName()+" "+tempChildNode.GetNodeName());
                                    exprTempNode.SetValue(FactValue<T>.Parse(replacedString));
                                    exprTempNode.SetEquation(FactValue<T>.Parse(replacedString));
                                }
                        }
                        else if(lt.Equals(LineType.COMPARISON))
                        {
                            tempNode = new ComparisonLine<T>(nextNThInString+" "+this.GetVariableName()+" "+tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                            FactValue<T> tempNodeFv = ((ComparisonLine<T>)tempNode).GetRHS(); 
                            if(tempNodeFv.GetFactValueType().Equals(FactValueType.STRING))
                            {
                                FactValue<T> tempFv = FactValue<T>.Parse(nextNThInString+" "+this.GetVariableName()+" "+tempNodeFv);
                                tempNode.SetValue(tempFv);
                            }
                        }
                        else if(lt.Equals(LineType.EXPR_CONCLUSION))
                        {
                            tempNode = new ExprConclusionLine<T>(nextNThInString+" "+this.GetVariableName()+" "+tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                        }
                    }
                    else
                    {
                        if(lt.Equals(LineType.VALUE_CONCLUSION) && parentNodeMap[parentNodeIdMap[originalParentId]].GetLineType().Equals(LineType.EXPR_CONCLUSION))
                        {
                                
                            ExprConclusionLine<T> exprTempNode = ((ExprConclusionLine<T>)thisNodeMap[thisNodeIdMap[modifiedParentId]]);
                            string replacedString = exprTempNode.GetEquation().GetValue().ToString().Replace(tempChildNode.GetNodeName(), nextNThInString+" "+this.GetVariableName()+" "+tempChildNode.GetNodeName());
                            exprTempNode.SetValue(FactValue<T>.Parse(replacedString));
                            exprTempNode.SetEquation(FactValue<T>.Parse(replacedString));
                        }
                    }
                    
                            
                    thisNodeMap.Add(tempNode.GetNodeName(), tempNode);
                    thisNodeIdMap.Add(tempNode.GetNodeId(), tempNode.GetNodeName());
                    tempDependencyList.Add(new Dependency<T>(thisNodeMap[thisNodeIdMap[modifiedParentId]], tempNode,parentDM.GetDependencyType(originalParentId, item)));
                    
                    CreateIterateNodeSetAux(parentDM, parentNodeMap, parentNodeIdMap, thisNodeMap, thisNodeIdMap, tempDependencyList, item, tempNode.GetNodeId(), nextNThInString);
                });
            }       
        }

       
        public NodeSet<T> GetIterateNodeSet()
        {
            return this.iterateNodeSet;
        }
        
        /*
         * this method is used when a givenList exists as a string
         */
        public void IterateFeedAnswers(string givenJsonString, NodeSet<T> parentNodeSet, AssessmentState<T> parentAst, Assessment<T> ass) // this method uses JSON object via jackson library
        {
            /*
             *      givenJsonString has to be in same format as Example otherwise the engine would NOT be able to enable 'IterateLine' node
             *      --------------------------- "givenJsonString" Format ----------------------------
             *      
             *      string givenJsonString = "{
             *                                  \"iterateLineVariableName\":
             *                                      [
             *                                        {
             *                                          \"1st iterateLineVariableName\":
             *                                              {
             *                                                \"1st iterateLineVariableName ruleNme1\":\"..value..\", 
             *                                                \"1st iterateLineVariableName ruleNme2\":\"..value..\"
             *                                              }
             *                                        },
             *                                        {
             *                                          \"2nd iterateLineVariableName\":
             *                                              {
             *                                                \"2nd iterateLineVariableName ruleName1\":\"..value..\",
             *                                                \"2nd iterateLineVariableName ruleName2\":\"..value..\"}
             *                                        }
             *                                      ]
             *                               }";
             *
             *     -----------------------------  "givenJsonString" Example ----------------------------
             *     string givenJsonString = "{
             *                                  \"service\":
             *                                      [
             *                                        {
             *                                          \"1st service\":
             *                                              {
             *                                                \"1st service period\":\"..value..\", 
             *                                                \"1st service type\":\"..value..\"
             *                                              }
             *                                        },
             *                                        {
             *                                          \"2nd service\":
             *                                              {
             *                                                \"2nd service period\":\"..value..\",
             *                                                \"2nd service type\":\"..value..\"}
             *                                        }
             *                                      ]
             *                               }";
            */
            JObject jObject = JObject.Parse(givenJsonString);
            List<JToken> serviceList = jObject.Property(this.variableName).ToList();
            this.givenListSize = serviceList.Count;

            if(this.iterateNodeSet == null)
            {
                this.iterateNodeSet = CreateIterateNodeSet(parentNodeSet);
                this.iterateIE = new InferenceEngine<T>(this.iterateNodeSet);
                if(this.iterateIE.GetAssessment() == null)
                {
                    this.iterateIE.SetAssessment(new Assessment<T>(this.iterateNodeSet, this.GetNodeName()));
                }
            } 
            while (!this.iterateIE.GetAssessmentState().GetWorkingMemory().ContainsKey(this.nodeName))
            {
                Node<T> nextQuestionNode = GetIterateNextQuestion(parentNodeSet, parentAst);
                string answer = "";
                Dictionary<string, FactValueType> questionFvtMap = this.iterateIE.FindTypeOfElementToBeAsked(nextQuestionNode);
                foreach (string question in this.iterateIE.GetQuestionsFromNodeToBeAsked(nextQuestionNode))
                {
                //    answer = jObject.GetValue(this.variableName)
                //                    .SelectToken
                //        jsonObj.get(this.variableName)
                //                    .get(nextQuestionNode.getVariableName().substring(0, nextQuestionNode.getVariableName().lastIndexOf(this.variableName) + this.variableName.length()))
                //                    .get(nextQuestionNode.getVariableName())
                //                    .asText().trim();

                    this.iterateIE.FeedAnswerToNode(nextQuestionNode, question, FactValue<T>.Parse(answer).GetValue() , questionFvtMap[question], this.iterateIE.GetAssessment());
                }

                Dictionary<string, FactValue<T>> iterateWorkingMemory = this.iterateIE.GetAssessmentState().GetWorkingMemory();
                Dictionary<String, FactValue<T>> parentWorkingMemory = parentAst.GetWorkingMemory();

                TranseferFactValue(iterateWorkingMemory, parentWorkingMemory);

            }
        }
        
        ///*
        // * this method is used when a givenList does NOT exist
        // */
        public void IterateFeedAnswers(Node<T> targetNode, string questionName, T nodeValue, FactValueType nodeValueType, NodeSet<T> parentNodeSet, AssessmentState<T> parentAst, Assessment<T> ass)
        {
            
            if(this.iterateNodeSet == null)
            {
                Node<T> firstIterateQuestionNode = parentNodeSet.GetNodeByNodeId(parentNodeSet.GetDependencyMatrix().GetToChildDependencyList(this.GetNodeId()).Min());
                if(questionName.Equals(firstIterateQuestionNode.GetNodeName()))
                {
                    this.givenListSize = Int32.Parse(nodeValue.ToString());
                }
                this.iterateNodeSet = CreateIterateNodeSet(parentNodeSet);
                this.iterateIE = new InferenceEngine<T>(this.iterateNodeSet);
                if(this.iterateIE.GetAssessment() == null)
                {
                    this.iterateIE.SetAssessment(new Assessment<T>(this.iterateNodeSet, this.GetNodeName()));
                }
            } 
            this.iterateIE.GetAssessment().SetNodeToBeAsked(targetNode);
            this.iterateIE.FeedAnswerToNode(targetNode, questionName, nodeValue, nodeValueType, this.iterateIE.GetAssessment());
            
            Dictionary<string,FactValue<T>> iterateWorkingMemory = this.iterateIE.GetAssessmentState().GetWorkingMemory();
            Dictionary<string,FactValue<T>> parentWorkingMemory = parentAst.GetWorkingMemory();

            TranseferFactValue(iterateWorkingMemory, parentWorkingMemory);
            
        }
        
        public void TranseferFactValue(Dictionary<string, FactValue<T>> workingMemory_one, Dictionary<string, FactValue<T>> workingMemory_two)
        {
            workingMemory_one.Keys.ToList().ForEach((key) =>
            {
                FactValue<T> tempFv = workingMemory_one[key];
                if (!workingMemory_two.ContainsKey(key))
                {
                    workingMemory_two.Add(key, tempFv);
                }
            });                
        }
        
        public Node<T> GetIterateNextQuestion(NodeSet<T> parentNodeSet, AssessmentState<T> parentAst)
        {
            if(this.iterateNodeSet == null && this.givenListSize != 0)
            {
                this.iterateNodeSet = CreateIterateNodeSet(parentNodeSet);
                this.iterateIE = new InferenceEngine<T>(this.iterateNodeSet);
                if(this.iterateIE.GetAssessment() == null)
                {
                    this.iterateIE.SetAssessment(new Assessment<T>(this.iterateNodeSet, this.GetNodeName()));
                }
            }

            Node<T> firstIterateQuestionNode = parentNodeSet.GetNodeByNodeId(parentNodeSet.GetDependencyMatrix().GetToChildDependencyList(this.GetNodeId()).Min());
            Node<T> questionNode = null;
                
            if(!parentAst.GetWorkingMemory().ContainsKey(this.value.GetValue().ToString())) // a list is not given yet so that the engine needs to find out more info.
            {

                if(!parentAst.GetWorkingMemory().ContainsKey(firstIterateQuestionNode.GetNodeName()))
                {
                    questionNode = firstIterateQuestionNode;
                }
                else 
                {
                    if(!CanBeSelfEvaluated(parentAst.GetWorkingMemory()))
                    {
                        questionNode = this.iterateIE.GetNextQuestion(this.iterateIE.GetAssessment());
                    }
                }
            }
            
                
                return questionNode;
        }
        

        public int FindNTh(Dictionary<string, FactValue<T>> workingMemory)
        {
            return Enumerable.Range(0, this.givenListSize + 1)
                             .Where((item) => workingMemory[Oridinal(item) + " " + this.variableName] != null)
                             .ToList().Count;                            
        }
        
        public String Oridinal(int i) 
        {
            String[] sufixes = new String[] { "th", "st", "nd", "rd", "th", "th", "th", "th", "th", "th" };
            switch (i % 100) {
                case 11:
                case 12:
                case 13:
                    return i + "th";
                default:
                    return i + sufixes[i % 10];
            }
        }
        
        
        public override void Initialisation(string parentText, Tokens tokens) {
            this.nodeName = parentText;
            this.numberOfTarget = tokens.tokensList[0];
            this.variableName = tokens.tokensList[1];
            int tokensStringListSize = tokens.tokensStringList.Count;
            string lastToken = tokens.tokensList[tokensStringListSize-1]; //this is a givenListName.
            string lastTokenString = tokens.tokensStringList[tokensStringListSize-1];
            this.SetValue(lastTokenString, lastToken);
            this.givenListName = lastToken;
            
        }

        public override LineType GetLineType() {
            return LineType.ITERATE;
        }


        public bool CanBeSelfEvaluated(Dictionary<string, FactValue<T>> workingMemory) 
        {
            bool canBeSelfEvaluated = false;
            if(this.iterateIE != null)
            {
                List<int> numberOfDeterminedSecondLevelNode = this.iterateIE.GetNodeSet().GetDependencyMatrix().GetToChildDependencyList(this.nodeId)
                                                                   .Where((i) => i != this.nodeId + 1)
                                                                   .Where((id) => workingMemory[this.iterateIE.GetNodeSet().GetNodeIdMap()[id]] != null && workingMemory[this.iterateIE.GetNodeSet().GetNodeIdMap()[id]].GetValue() != null)
                                                                   .ToList();
                            


                if(this.givenListSize == numberOfDeterminedSecondLevelNode.Count && this.iterateIE.HasAllMandatoryChildAnswered(this.nodeId))
                {
                    canBeSelfEvaluated = true;
                }
            }
            
            
            
            return canBeSelfEvaluated;
        }

        
        public override FactValue<T> SelfEvaluate(Dictionary<string, FactValue<T>> workingMemory, Jint.Engine nashorn) 
        {
            
            int numberOfTrueChildren = NumberOfTrueChildren(workingMemory);
            int sizeOfGivenList = this.givenListSize;
            FactBooleanValue<T> fbv = null;
            switch(this.numberOfTarget)
            {
                case "ALL":
                    if(numberOfTrueChildren == sizeOfGivenList)
                    {
                        fbv = FactBooleanValue<T>.Parse(true);
                    }
                    else
                    {
                        fbv = FactBooleanValue<T>.Parse(false);
                    }
                    break;
                case "NONE":
                    if(numberOfTrueChildren == 0)
                    {
                        fbv = FactBooleanValue<T>.Parse(true);
                    }
                    else
                    {
                        fbv = FactBooleanValue<T>.Parse(false);
                    }
                    break;
                case "SOME":
                    if(numberOfTrueChildren > 0)
                    {
                        fbv = FactBooleanValue<T>.Parse(true);
                    }
                    else
                    {
                        fbv = FactBooleanValue<T>.Parse(false);
                    }
                    break;
                default:
                    if(numberOfTrueChildren == Int32.Parse(this.numberOfTarget))
                    {
                        fbv = FactBooleanValue<T>.Parse(true);
                    }
                    else
                    {
                        fbv = FactBooleanValue<T>.Parse(false);
                    }
                    break;
            }
            return fbv;
        }
        
        public int NumberOfTrueChildren(Dictionary<string, FactValue<T>> workingMemory)
        {
            return this.iterateIE.GetNodeSet().GetDependencyMatrix().GetToChildDependencyList(this.nodeId)
                            .Where((i) => i != this.nodeId + 1)
                            .Where((id) => workingMemory[this.iterateIE.GetNodeSet().GetNodeIdMap()[id]].GetValue().ToString().ToLower().Equals("true"))
                            .ToList().Count;

        }
    }
}
