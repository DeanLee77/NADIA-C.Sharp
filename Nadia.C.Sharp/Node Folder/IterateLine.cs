using System;
using System.Collections.Generic;
using System.Linq;
using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.InferenceEngineFolder;
using Nadia.C.Sharp.RuleParserFolder;
using Newtonsoft.Json.Linq;

namespace Nadia.C.Sharp.NodeFolder
{
    public class IterateLine: Node
    {
        private String numberOfTarget;
        private NodeSet iterateNodeSet;
        private String givenListName;
        private int givenListSize;
        private InferenceEngine iterateIE;

        

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
        public NodeSet CreateIterateNodeSet(NodeSet parentNodeSet)
        {

            DependencyMatrix parentDM = parentNodeSet.GetDependencyMatrix();
            Dictionary<string, Node> parentNodeMap = parentNodeSet.GetNodeMap();
            Dictionary<int?, string> parentNodeIdMap = parentNodeSet.GetNodeIdMap();
            
            
            Dictionary<string, Node> thisNodeMap = new Dictionary<string, Node>();
            Dictionary<int?, string> thisNodeIdMap = new Dictionary<int?, string>();
            List<Dependency> tempDependencyList = new List<Dependency>();
            NodeSet newNodeSet = new NodeSet();

            thisNodeMap.Add(this.nodeName, this);
            thisNodeIdMap.Add(this.nodeId, this.nodeName);
            Enumerable.Range(1, this.givenListSize).ToList().ForEach((nth) =>
            {
                parentDM.GetToChildDependencyList(this.nodeId).ForEach((item) =>
                {
                    if (this.GetNodeId() + 1 != item) // not first question id
                    {
                        Node tempChildNode = parentNodeMap[parentNodeIdMap[item]];
                        LineType lineType = tempChildNode.GetLineType();

                        Node tempNode = null;
                        string nextNThInString = Oridinal(nth);

                        if (lineType.Equals(LineType.VALUE_CONCLUSION))
                        {
                            tempNode = new ValueConclusionLine(nextNThInString + " " + this.GetVariableName() + " " + tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                        }
                        else if (lineType.Equals(LineType.COMPARISON))
                        {
                            tempNode = new ComparisonLine(nextNThInString + " " + this.GetVariableName() + " " + tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                            FactValue tempNodeFv = ((ComparisonLine)tempNode).GetRHS();
                            if (tempNodeFv.GetFactValueType().Equals(FactValueType.STRING))
                            {
                                FactValue tempFv = FactValue.Parse(nextNThInString + " " + this.GetVariableName() + " " + FactValue.GetValueInString(FactValueType.STRING, tempNodeFv));
                                tempNode.SetValue(tempFv);
                            }
                        }
                        else if (lineType.Equals(LineType.EXPR_CONCLUSION))
                        {
                            tempNode = new ExprConclusionLine(nextNThInString + " " + this.GetVariableName() + " " + tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                        }


                        thisNodeMap.Add(tempNode.GetNodeName(), tempNode);
                        thisNodeIdMap.Add(tempNode.GetNodeId(), tempNode.GetNodeName());
                        tempDependencyList.Add(new Dependency(this, tempNode, parentDM.GetDependencyType(this.nodeId, item)));

                        CreateIterateNodeSetAux(parentDM, parentNodeMap, parentNodeIdMap, thisNodeMap, thisNodeIdMap, tempDependencyList, item, tempNode.GetNodeId(), nextNThInString);

                    }
                    else // first question id
                    {
                        Node firstIterateQuestionNode = parentNodeSet.GetNodeByNodeId(parentNodeSet.GetDependencyMatrix().GetToChildDependencyList(this.GetNodeId()).Min());
                        if(!thisNodeMap.ContainsKey(firstIterateQuestionNode.GetNodeName()))
                        {
                            thisNodeMap.Add(firstIterateQuestionNode.GetNodeName(), firstIterateQuestionNode);
                            thisNodeIdMap.Add(item, firstIterateQuestionNode.GetNodeName());
                            tempDependencyList.Add(new Dependency(this, firstIterateQuestionNode, parentDM.GetDependencyType(this.nodeId, item)));
                        }

                    }
                });
            });

           
            
            int numberOfRules = Node.GetStaticNodeId();
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
            newNodeSet.SetNodeSortedList(TopoSort.DfsTopoSort(thisNodeMap, thisNodeIdMap, dependencyMatrix));
    //      newNodeSet.getNodeSortedList().stream().forEachOrdered(item->System.out.println(item.getNodeId()+"="+item.getNodeName()));
            return newNodeSet;
        
        }
        
        public void CreateIterateNodeSetAux(DependencyMatrix parentDM, Dictionary<string, Node> parentNodeMap, Dictionary<int?, string> parentNodeIdMap, Dictionary<string, Node> thisNodeMap, Dictionary<int?, string> thisNodeIdMap, List<Dependency> tempDependencyList, int originalParentId, int modifiedParentId, string nextNThInString)
        {
            List<int> childDependencyList = parentDM.GetToChildDependencyList(originalParentId);

            if(childDependencyList.Count > 0)
            {
                childDependencyList.ForEach((item) =>{
                    Node tempChildNode = parentNodeMap[parentNodeIdMap[item]];
                    LineType lt = tempChildNode.GetLineType();

                    Node tempNode;
                    thisNodeMap.TryGetValue(nextNThInString + " " + this.GetVariableName() + " " + tempChildNode.GetNodeName(), out tempNode);
                    if(tempNode == null)
                    {
                        if(lt.Equals(LineType.VALUE_CONCLUSION))
                        {
                                tempNode = new ValueConclusionLine(nextNThInString+" "+this.GetVariableName()+" "+tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                                
                                if(parentNodeMap[parentNodeIdMap[originalParentId]].GetLineType().Equals(LineType.EXPR_CONCLUSION))
                                {
                                    ExprConclusionLine exprTempNode = thisNodeMap[thisNodeIdMap[modifiedParentId]] as ExprConclusionLine;
                                    string replacedString = FactValue.GetValueInString(exprTempNode.GetEquation().GetFactValueType(), exprTempNode.GetEquation()).Replace(tempChildNode.GetNodeName(), nextNThInString+" "+this.GetVariableName()+" "+tempChildNode.GetNodeName());
                                    exprTempNode.SetValue(FactValue.Parse(replacedString));
                                    exprTempNode.SetEquation(FactValue.Parse(replacedString));
                                }
                        }
                        else if(lt.Equals(LineType.COMPARISON))
                        {
                            tempNode = new ComparisonLine(nextNThInString+" "+this.GetVariableName()+" "+tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                            FactValue tempNodeFv = ((ComparisonLine)tempNode).GetRHS(); 
                            if(tempNodeFv.GetFactValueType().Equals(FactValueType.STRING))
                            {
                                FactValue tempFv = FactValue.Parse(nextNThInString+" "+this.GetVariableName()+" "+tempNodeFv);
                                tempNode.SetValue(tempFv);
                            }
                        }
                        else if(lt.Equals(LineType.EXPR_CONCLUSION))
                        {
                            tempNode = new ExprConclusionLine(nextNThInString+" "+this.GetVariableName()+" "+tempChildNode.GetNodeName(), tempChildNode.GetTokens());
                        }
                    }
                    else
                    {
                        if(lt.Equals(LineType.VALUE_CONCLUSION) && parentNodeMap[parentNodeIdMap[originalParentId]].GetLineType().Equals(LineType.EXPR_CONCLUSION))
                        {
                                
                            ExprConclusionLine exprTempNode = ((ExprConclusionLine)thisNodeMap[thisNodeIdMap[modifiedParentId]]);
                            string replacedString = FactValue.GetValueInString(exprTempNode.GetEquation().GetFactValueType(), exprTempNode.GetEquation()).Replace(tempChildNode.GetNodeName(), nextNThInString+" "+this.GetVariableName()+" "+tempChildNode.GetNodeName());
                            exprTempNode.SetValue(FactValue.Parse(replacedString));
                            exprTempNode.SetEquation(FactValue.Parse(replacedString));
                        }
                    }
                    
                    if(!thisNodeMap.ContainsKey(tempNode.GetNodeName()))
                    {
                        thisNodeMap.Add(tempNode.GetNodeName(), tempNode);
                        thisNodeIdMap.Add(tempNode.GetNodeId(), tempNode.GetNodeName());
                    }

                    tempDependencyList.Add(new Dependency(thisNodeMap[thisNodeIdMap[modifiedParentId]], tempNode, parentDM.GetDependencyType(originalParentId, item)));

                    CreateIterateNodeSetAux(parentDM, parentNodeMap, parentNodeIdMap, thisNodeMap, thisNodeIdMap, tempDependencyList, item, tempNode.GetNodeId(), nextNThInString);

                });
            }       
        }

       
        public NodeSet GetIterateNodeSet()
        {
            return this.iterateNodeSet;
        }
        
        /*
         * this method is used when a givenList exists as a string
         */
        public void IterateFeedAnswers(string givenJsonString, NodeSet parentNodeSet, AssessmentState parentAst, Assessment ass) // this method uses JSON object via jackson library
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
                this.iterateIE = new InferenceEngine(this.iterateNodeSet);
                if(this.iterateIE.GetAssessment() == null)
                {
                    this.iterateIE.SetAssessment(new Assessment(this.iterateNodeSet, this.GetNodeName()));
                }
            } 
            while (!this.iterateIE.GetAssessmentState().GetWorkingMemory().ContainsKey(this.nodeName))
            {
                Node nextQuestionNode = GetIterateNextQuestion(parentNodeSet, parentAst);
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

                    this.iterateIE.FeedAnswerToNode(nextQuestionNode, question, FactValue.Parse(answer) , this.iterateIE.GetAssessment());
                }

                Dictionary<string, FactValue> iterateWorkingMemory = this.iterateIE.GetAssessmentState().GetWorkingMemory();
                Dictionary<String, FactValue> parentWorkingMemory = parentAst.GetWorkingMemory();

                TranseferFactValue(iterateWorkingMemory, parentWorkingMemory);

            }
        }
        
        ///*
        // * this method is used when a givenList does NOT exist
        // */
        public void IterateFeedAnswers(Node targetNode, string questionName, FactValue nodeValue, NodeSet parentNodeSet, AssessmentState parentAst, Assessment ass)
        {
            
            if(this.iterateNodeSet == null)
            {
                Node firstIterateQuestionNode = parentNodeSet.GetNodeByNodeId(parentNodeSet.GetDependencyMatrix().GetToChildDependencyList(this.GetNodeId()).Min());
                if(questionName.Equals(firstIterateQuestionNode.GetNodeName()))
                {
                    this.givenListSize = Int32.Parse(FactValue.GetValueInString(nodeValue.GetFactValueType(), nodeValue));
                }
                this.iterateNodeSet = CreateIterateNodeSet(parentNodeSet);
                this.iterateIE = new InferenceEngine(this.iterateNodeSet);
                if(this.iterateIE.GetAssessment() == null)
                {
                    this.iterateIE.SetAssessment(new Assessment(this.iterateNodeSet, this.GetNodeName()));
                }
            } 
            this.iterateIE.GetAssessment().SetNodeToBeAsked(targetNode);
            this.iterateIE.FeedAnswerToNode(targetNode, questionName, nodeValue, this.iterateIE.GetAssessment());
            
            Dictionary<string,FactValue> iterateWorkingMemory = this.iterateIE.GetAssessmentState().GetWorkingMemory();
            Dictionary<string,FactValue> parentWorkingMemory = parentAst.GetWorkingMemory();

            TranseferFactValue(iterateWorkingMemory, parentWorkingMemory);
            
        }
        
        public void TranseferFactValue(Dictionary<string, FactValue> workingMemory_one, Dictionary<string, FactValue> workingMemory_two)
        {
            workingMemory_one.Keys.ToList().ForEach((key) =>
            {
                FactValue tempFv = workingMemory_one[key];
                if (!workingMemory_two.ContainsKey(key))
                {
                    workingMemory_two.Add(key, tempFv);
                }
            });                
        }
        
        public Node GetIterateNextQuestion(NodeSet parentNodeSet, AssessmentState parentAst)
        {
            if(this.iterateNodeSet == null && this.givenListSize != 0)
            {
                this.iterateNodeSet = CreateIterateNodeSet(parentNodeSet);
                this.iterateIE = new InferenceEngine(this.iterateNodeSet);
                if(this.iterateIE.GetAssessment() == null)
                {
                    this.iterateIE.SetAssessment(new Assessment(this.iterateNodeSet, this.GetNodeName()));
                }
            }

            Node firstIterateQuestionNode = parentNodeSet.GetNodeByNodeId(parentNodeSet.GetDependencyMatrix().GetToChildDependencyList(this.GetNodeId()).Min());
            Node questionNode = null;
                
            if(!parentAst.GetWorkingMemory().ContainsKey(FactValue.GetValueInString(this.value.GetFactValueType(), this.value))) // a list is not given yet so that the engine needs to find out more info.
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
        

        public int FindNTh(Dictionary<string, FactValue> workingMemory)
        {
            return Enumerable.Range(1, this.givenListSize)
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


        public bool CanBeSelfEvaluated(Dictionary<string, FactValue> workingMemory) 
        {
            bool canBeSelfEvaluated = false;
            if(this.iterateIE != null)
            {
                FactValue outFactValue = null;
                List<int> numberOfDeterminedSecondLevelNode = this.iterateIE.GetNodeSet().GetDependencyMatrix().GetToChildDependencyList(this.nodeId)
                                                                  .Where((i) => i != this.nodeId + 1)
                                                                  .Where((id) => workingMemory.TryGetValue(this.iterateIE.GetNodeSet().GetNodeIdMap()[id], out outFactValue) && outFactValue != null && FactValue.GetValueInString(outFactValue.GetFactValueType(), outFactValue) != null)
                                                                  .ToList();
                            


                if(this.givenListSize == numberOfDeterminedSecondLevelNode.Count && this.iterateIE.HasAllMandatoryChildAnswered(this.nodeId))
                {
                    canBeSelfEvaluated = true;
                }
            }
            
            
            
            return canBeSelfEvaluated;
        }

        
        public override FactValue SelfEvaluate(Dictionary<string, FactValue> workingMemory, Jint.Engine nashorn) 
        {
            
            int numberOfTrueChildren = NumberOfTrueChildren(workingMemory);
            int sizeOfGivenList = this.givenListSize;
            FactBooleanValue fbv = null;
            switch(this.numberOfTarget)
            {
                case "ALL":
                    if(numberOfTrueChildren == sizeOfGivenList)
                    {
                        fbv = FactValue.Parse(true);
                    }
                    else
                    {
                        fbv = FactValue.Parse(false);
                    }
                    break;
                case "NONE":
                    if(numberOfTrueChildren == 0)
                    {
                        fbv = FactValue.Parse(true);
                    }
                    else
                    {
                        fbv = FactValue.Parse(false);
                    }
                    break;
                case "SOME":
                    if(numberOfTrueChildren > 0)
                    {
                        fbv = FactValue.Parse(true);
                    }
                    else
                    {
                        fbv = FactValue.Parse(false);
                    }
                    break;
                default:
                    if(numberOfTrueChildren == Int32.Parse(this.numberOfTarget))
                    {
                        fbv = FactValue.Parse(true);
                    }
                    else
                    {
                        fbv = FactValue.Parse(false);
                    }
                    break;
            }
            return fbv;
        }
        
        public int NumberOfTrueChildren(Dictionary<string, FactValue> workingMemory)
        {
            return this.iterateIE.GetNodeSet().GetDependencyMatrix().GetToChildDependencyList(this.nodeId)
                            .Where((i) => i != this.nodeId + 1)
                       .Where((id) => FactValue.GetValueInString(workingMemory[this.iterateIE.GetNodeSet().GetNodeIdMap()[id]].GetFactValueType(), workingMemory[this.iterateIE.GetNodeSet().GetNodeIdMap()[id]]).ToLower().Equals("true"))
                            .ToList().Count;

        }
    }
}
