using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.NodeFolder;
using Nadia.C.Sharp.RuleParserFolder;

namespace Nadia.C.Sharp.RuleParserFolder
{
    public class RuleSetParser<T> : IScanFeeder<T>
    {
        //  enum LineType {META, VALUE_CONCLUSION, EXPR_CONCLUSION, COMPARISON, WARNING}
        /*
         * patterns are as follows;
         * U : upper case
         * L : lower case
         * M : mixed case
         * No: number
         * Da: date
         * Ha: hash
         * Url: url
         * Id: UUID
         * C : CALC (this is a keyword)
         * De: decimal
         * Q : quotation mark
         * 
         */


        Regex META_PATTERN_MATCHER = new Regex(@"(^U)([MLU]*)([(No)(Da)ML(De)(Ha)(U(rl)?)(Id)]*$)");
        Regex VALUE_MATCHER = new Regex(@"(^[LM]+)(U)?([MLQ(No)(Da)(De)(Ha)(Url)(Id)]*$)(?!C)");
        Regex EXPRESSION_CONCLUSION_MATCHER = new Regex(@"(^[LM(Da)]+)(U)(C)");
        Regex COMPARISON_MATCHER = new Regex(@"(^[MLU(Da)]+)(O)([MLUQ(No)(Da)(De)(Ha)(Url)(Id)]*$)");
        Regex ITERATE_MATCHER = new Regex(@"(^[MLU(No)(Da)]+)(I)([MLU]+$)");
        Regex WARNING_MATCHER = new Regex(@"WARNING");
        NodeSet<T> nodeSet = new NodeSet<T>();
        List<Dependency<T>> dependencyList = new List<Dependency<T>>();



        public void HandleParent(string parentText, int lineNumber)
        {

            Node<T> data = nodeSet.GetNodeMap()[parentText];

            if (data == null)
            {

                Tokens tokens = Tokenizer.GetTokens(parentText);

                Regex[] matchPatterns = { META_PATTERN_MATCHER, VALUE_MATCHER, EXPRESSION_CONCLUSION_MATCHER, WARNING_MATCHER };

                for (int i = 0; i < matchPatterns.Length; i++)
                {
                    //matcher = Regex.Match(tokens.tokensString, matchPatterns[i]);
                    Regex regex = matchPatterns[i];
                    Match match = regex.Match(tokens.tokensString);
                    if (match.Success)
                    {
                        string variableName;
                        Node<T> tempNode;
                        List<string> possibleParentNodeKeyList;
                        switch(i){
                            case 3:  //warningMatcher case
                                HandleWarning(parentText);
                                break;
                            case 0:  //metaMatcher case
                                data = new MetadataLine<T>(parentText, tokens);

                                if (data.GetFactValue().GetValue().Equals("WARNING"))
                                {
                                    HandleWarning(parentText);
                                }
                                break;
                            case 1: //valueConclusionLine case
                                data = new ValueConclusionLine<T>(parentText, tokens);
                                if (match.Groups[2] != null
                                    || (tokens.tokensString.Equals("L") || tokens.tokensString.Equals("LM") || tokens.tokensString.Equals("ML") || tokens.tokensString.Equals("M")))
                                {
                                    variableName = data.GetVariableName();
                                    tempNode = data;
                                    /*
                                     * following lines are to look for any nodes having a its nodeName with any operators due to the reason that
                                     * the node could be used to define a node previously used as a child node for other nodes
                                     */
                                    possibleParentNodeKeyList = nodeSet.GetNodeMap().Keys.Where(key=> Regex.Match(key, "(.+)?(\\s[<>=]+\\s?)?(" + variableName + ")(\\s[<>=]+)*(.(?!(IS)))*(.*(IS IN LIST).*)*").Success).ToList();
                                    if (possibleParentNodeKeyList.Count != 0)
                                    {
                                        possibleParentNodeKeyList.ForEach(item => {
                                            this.dependencyList.Add(new Dependency<T>(nodeSet.GetNodeMap()[item], tempNode, DependencyType.GetOr())); //Dependency Type :OR
                                        });
                                    }
                                }   
                                if(data.GetFactValue().GetValue().Equals("WARNING"))
                                {
                                    HandleWarning(parentText);
                                }
                                break;
                            case 2: //exprConclusionMatcher case 
                                data = new ExprConclusionLine<T>(parentText, tokens);
                                
                                variableName = data.GetVariableName();
                                tempNode = data;
                                /*
                                 * following lines are to look for any nodes having a its nodeName with any operators due to the reason that
                                 * the exprConclusion node could be used to define another node as a child node for other nodes if the variableName of exprConclusion node is mentioned somewhere else.
                                 * However, it is excluding nodes having 'IS' keyword because if it has the keyword then it should have child nodes to define the node otherwise the entire rule set has NOT been written in correct way
                                 */
                                possibleParentNodeKeyList = nodeSet.GetNodeMap().Keys.Where(key => Regex.Match(key, "(.+)?(\\s[<>=]+\\s?)?(" + variableName + ")(\\s[<>=]+)*(.(?!(IS)))*(.*(IS IN LIST).*)*").Success).ToList();
                                if(possibleParentNodeKeyList.Count!= 0)
                                {
                                    possibleParentNodeKeyList.ForEach(item => {
                                        this.dependencyList.Add(new Dependency<T>(nodeSet.GetNodeMap()[item], tempNode, DependencyType.GetOr())); //Dependency Type :OR
                                    });
                                }
                                if(data.GetFactValue().GetValue().Equals("WARNING"))
                                {
                                    HandleWarning(parentText);
                                }
                                break;
                            default:
                                HandleWarning(parentText);
                                break;  
                                
                        }
                        data.SetNodeLine(lineNumber);
                        if (data.GetLineType().Equals(LineType.META))
                        {
                            if (((MetadataLine<T>)data).GetMetaType().Equals(MetaType.INPUT))
                            {
                                this.nodeSet.GetInputMap().Add(data.GetVariableName(), data.GetFactValue());
                            }
                            else if (((MetadataLine<T>)data).GetMetaType().Equals(MetaType.FIXED))
                            {
                                this.nodeSet.GetFactMap().Add(data.GetVariableName(), data.GetFactValue());
                            }
                        }
                        else
                        {
                            this.nodeSet.GetNodeMap().Add(data.GetNodeName(), data);
                            this.nodeSet.GetNodeIdMap().Add(data.GetNodeId(), data.GetNodeName());
                        }
                        break;
                    }                   
                }
            }    
        }           
    
        public void HandleChild(string parentText, string childText, string firstKeywordsGroup, int lineNumber)
        {

            /*
             * the reason for using '*' at the last group of pattern within comparison is that 
             * the last group contains No, Da, De, Ha, Url, Id. 
             * In order to track more than one character within the square bracket of last group '*'(Matches 0 or more occurrences of the preceding expression) needs to be used.
             * 
             */
            int dependencyType = 0;

            // is 'ITEM' child line
            if (Regex.Match(childText, "(ITEM)(.*)").Success)
            {
                if (!Regex.Match(parentText, "(.*)(AS LIST)").Success)
                {
                    HandleWarning(childText);
                    return;
                }

                // is an indented item child

                childText = childText.Remove(childText.IndexOf("ITEM", StringComparison.CurrentCulture), "ITEM".Length).Trim();

                MetaType? metaType = null;
                if (Regex.Match(parentText, "^(INPUT)(.*)").Success)
                {
                    metaType = MetaType.INPUT;
                }
                else if (Regex.Match(parentText, "^(FIXED)(.*)").Success)
                {
                    metaType = MetaType.FIXED;
                }
                HandleListItem(parentText, childText, metaType);
            }
            else  // is 'A-statement', 'A IS B', 'A <= B', or 'A IS CALC (B * C)' child line
            {
                if (Regex.Match(firstKeywordsGroup, "^(AND\\s?)(.*)").Success)
                {
                    dependencyType = HandleNotKnownManOptPos(firstKeywordsGroup, DependencyType.GetAnd()); // 8-AND | 1-KNOWN? 2-NOT? 64-MANDATORY? 32-OPTIONALLY? 16-POSSIBLY? 
                }
                else if (Regex.Match(firstKeywordsGroup, "^(OR\\s?)(.*)").Success)
                {
                    dependencyType = HandleNotKnownManOptPos(firstKeywordsGroup, DependencyType.GetOr()); // 4-OR | 1-KNOWN? 2-NOT? 64-MANDATORY? 32-OPTIONALLY? 16-POSSIBLY? 
                }
                else if (Regex.Match(firstKeywordsGroup, "^(WANTS)").Success)
                {
                    dependencyType = DependencyType.GetOr(); // 4-OR
                }
                else if (Regex.Match(firstKeywordsGroup, "^(NEEDS)").Success)
                {
                    dependencyType = DependencyType.GetMandatory() | DependencyType.GetAnd();  //  8-AND | 64-MANDATORY
                }


                /*
                 * the keyword of 'AND' or 'OR' should be removed individually. 
                 * it should NOT be removed by using firstToken string in Tokens.tokensList.get(0)
                 * because firstToken string may have something else. 
                 * (e.g. string: 'AND NOT ALL Males' name should sound Male', then Token string will be 'UMLM', and 'U' contains 'AND NOT ALL'.
                 * so if we used 'firstToken string' to remove 'AND' in this case as 'string.replace(firstTokenString)' 
                 * then it will remove 'AND NOT ALL' even we only need to remove 'AND' 
                 * 
                 */


                Node<T> data = nodeSet.GetNodeMap()[childText];
                Tokens tokens = Tokenizer.GetTokens(childText);

                if (data == null)
                {
                    //              valueConclusionMatcher =Pattern.compile("(^U)([LMU(Da)(No)(De)(Ha)(Url)(Id)]+$)"); // child statement for ValueConclusionLine starts with AND(OR), AND MANDATORY(OPTIONALLY, POSSIBLY) or AND (MANDATORY) (NOT) KNOWN

                    Regex[] matchPatterns = { VALUE_MATCHER, COMPARISON_MATCHER, ITERATE_MATCHER, EXPRESSION_CONCLUSION_MATCHER, WARNING_MATCHER };
                    Node<T> tempNode;
                    List<string> possibleChildNodeKeyList;

                    for (int i = 0; i < matchPatterns.Length; i++)
                    {
                        
                        Regex regex = matchPatterns[i];
                        Match match = regex.Match(tokens.tokensString);

                        if (match.Success)
                        {
                            switch (i)
                            {
                                case 4:  // warningMatcher case
                                    HandleWarning(childText);
                                    break;
                                case 0:  // valueConclusionMatcher case
                                    data = new ValueConclusionLine<T>(childText, tokens);

                                    tempNode = data;
                                    possibleChildNodeKeyList = nodeSet.GetNodeMap().Keys.Where(key=> Regex.Match(key, "(^" + tempNode.GetVariableName() + ")(.(IS(?!(\\sIN LIST))).*)*").Success).ToList();

                                    if (possibleChildNodeKeyList.Count != 0)
                                    {
                                        possibleChildNodeKeyList.ForEach(item => {
                                            this.dependencyList.Add(new Dependency<T>(tempNode, nodeSet.GetNodeMap()[item], DependencyType.GetOr())); //Dependency Type :OR
                                        });
                                    }

                                    if (data.GetFactValue().GetValue().Equals("WARNING"))
                                    {
                                        HandleWarning(parentText);
                                    }
                                    break;
                                case 1:  // comparisonMatcher case
                                    data = new ComparisonLine<T>(childText, tokens);

                                    FactValueType rhsType = ((ComparisonLine<T>)data).GetRHS().GetFactValueType();
                                    string rhsString = ((ComparisonLine<T>)data).GetRHS().GetValue().ToString();
                                    string lhsString = ((ComparisonLine<T>)data).GetLHS();
                                    tempNode = data;
                                    possibleChildNodeKeyList = rhsType.Equals(FactValueType.STRING) ?
                                                                nodeSet.GetNodeMap().Keys.Where(key => Regex.Match(key, "(^" + lhsString + ")(.(IS(?!(\\sIN LIST))).*)*").Success || Regex.Match(key, "(^" + rhsString + ")(.(IS(?!(\\sIN LIST))).*)*").Success).ToList()
                                                                :
                                                                nodeSet.GetNodeMap().Keys.Where(key => Regex.Match(key,"(^" + lhsString + ")(.(IS(?!(\\sIN LIST))).*)*").Success).ToList();

                                    if (possibleChildNodeKeyList.Count != 0)
                                    {
                                        possibleChildNodeKeyList.ForEach(item => {
                                            this.dependencyList.Add(new Dependency<T>(tempNode, nodeSet.GetNodeMap()[item], DependencyType.GetOr())); //Dependency Type :OR
                                        });
                                    }

                                    if (data.GetFactValue().GetValue().Equals("WARNING"))
                                    {
                                        HandleWarning(parentText);
                                    }
                                    break;
                                case 2:  // comparisonMatcher case
                                    data = new IterateLine<T>(childText, tokens);
                                    if (data.GetFactValue().GetValue().Equals("WARNING"))
                                    {
                                        HandleWarning(parentText);
                                    }
                                    break;
                            case 3: //exprConclusionMatcher case
                                data = new ExprConclusionLine<T>(childText, tokens);

                                /*
                                 * In this case, there is no mechanism to find possible parent nodes.
                                 * I have brought 'local variable' concept for this case due to it may massed up with structuring node dependency tree with topological sort
                                 * If ExprConclusion node is used as a child, then it means that this node is a local node which has to be strictly bound to its parent node only.  
                                 */

                                if (data.GetFactValue().GetValue().Equals("WARNING"))
                                {
                                    HandleWarning(parentText);
                                }
                                break;

                            }
                            data.SetNodeLine(lineNumber);
                            this.nodeSet.GetNodeMap().Add(data.GetNodeName(), data);
                            this.nodeSet.GetNodeIdMap().Add(data.GetNodeId(), data.GetNodeName());
                            break;
                        }
                    }
                }

                this.dependencyList.Add(new Dependency<T>(this.nodeSet.GetNode(parentText), data, dependencyType));
            }
        
        }


        public void HandleListItem(string parentText, string itemText, MetaType? metaType)
        {

            Tokens tokens = Tokenizer.GetTokens(itemText);
            FactValue<T> fv;
            if(tokens.tokensString.Equals("Da"))
            {
                
                DateTime factValueInDate;
                DateTime.TryParseExact(itemText, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out factValueInDate);
                fv = FactValue<T>.Parse(factValueInDate);
            }
            else if(tokens.tokensString.Equals("De"))
            {
                fv = FactValue<T>.Parse(Double.Parse(itemText));
            }
            else if(tokens.tokensString.Equals("No"))
            {
                fv = FactValue<T>.Parse(Int32.Parse(itemText));
            }
            else if(tokens.tokensString.Equals("Ha"))
            {
                fv = FactValue<T>.ParseHash(itemText);
            }
            else if(tokens.tokensString.Equals("Url"))
            {
                fv = FactValue<T>.ParseURL(itemText);
            }
            else if(tokens.tokensString.Equals("Id"))
            {
                fv = FactValue<T>.ParseUUID(itemText);
            }
            else if(Regex.Match(itemText, "FfAaLlSsEe").Success||Regex.Match(itemText, "TtRrUuEe").Success)
            {
                fv =FactValue<T>.Parse(Boolean.Parse(itemText));
            }
            else
            {
                fv = FactValue<T>.Parse(itemText);
            }
            String stringToGetFactValue = (parentText.Substring(5, parentText.IndexOf("AS", StringComparison.CurrentCulture))).Trim();
            if(metaType.Equals(MetaType.INPUT))
            {
                (this.nodeSet.GetInputMap()[stringToGetFactValue].GetValue() as List<FactValue<T>>).Add(fv);
            }
            else if(metaType.Equals(MetaType.FIXED))
            {
                (this.nodeSet.GetFactMap()[stringToGetFactValue].GetValue() as List<FactValue<T>>).Add(fv);
            }
    
        }

        public NodeSet<T> GetNodeSet()
        {
            return this.nodeSet;
        }

        public string HandleWarning(string parentText)
        {
            return parentText + ": rule format is not matched. Please check the format again";
        }

        /*
         * this method is to create virtual nodes where a certain node has 'AND' or 'MANDATORY_AND', and 'OR' children at the same time.
         * when a virtual node is created, all 'AND' children should be connected to the virtual node as 'AND' children
         * and the virtual node should be a 'OR' child of the original parent node 
         */
        public Dictionary<string, Node<T>> handlingVirtualNode(List<Dependency<T>> dependencyList)
        {

            Dictionary<string, Node<T>> virtualNodeMap = new Dictionary<string, Node<T>>();


            nodeSet.GetNodeMap().Values.ToList().ForEach((node)=>{
                virtualNodeMap.Add(node.GetNodeName(), node);
                List< Dependency<T>> dpList = dependencyList.Where(dp => node.GetNodeName().Equals(dp.GetParentNode().GetNodeName())).ToList();
                                   

                /*
                 * need to handle Mandatory, optionally, possibly NodeOptions
                 */
                int and = 0;
                int mandatoryAnd = 0;
                int or = 0;
                if (dpList.Count != 0)
                {
                    foreach (Dependency<T> dp in dpList) //can this for each loop be converted to dpList.stream().forEachOrdered() ?
                    {
                        if ((dp.GetDependencyType() & DependencyType.GetAnd()) == DependencyType.GetAnd())
                        {
                            and++;
                            if (dp.GetDependencyType() == (DependencyType.GetMandatory() | DependencyType.GetAnd()))
                            {
                                mandatoryAnd++;
                            }
                        }
                        else if ((dp.GetDependencyType() & DependencyType.GetOr()) == DependencyType.GetOr())
                        {
                            or++;
                        }
                    }
                    bool hasAndOr = (and > 0 && or > 0) ? true : false;
                    if (hasAndOr)
                    {

                        string parentNodeOfVirtualNodeName = node.GetNodeName();
                        Node<T> virtualNode = new ValueConclusionLine<T>("VirtualNode-" + parentNodeOfVirtualNodeName, Tokenizer.GetTokens("VirtualNode-" + parentNodeOfVirtualNodeName));
                        this.nodeSet.GetNodeIdMap().Add(virtualNode.GetNodeId(), "VirtualNode-" + parentNodeOfVirtualNodeName);
                        virtualNodeMap.Add("VirtualNode-" + parentNodeOfVirtualNodeName, virtualNode);
                        if (mandatoryAnd > 0)
                        {
                            dependencyList.Add(new Dependency<T>(node, virtualNode, (DependencyType.GetMandatory() | DependencyType.GetOr())));
                        }
                        else
                        {
                            dependencyList.Add(new Dependency<T>(node, virtualNode, DependencyType.GetOr()));
                        }
                        dpList.Where(dp =>dp.GetDependencyType() == DependencyType.GetAnd() || dp.GetDependencyType() == (DependencyType.GetMandatory() | DependencyType.GetAnd()))
                              .ToList().ForEach(dp=>dp.SetParentNode(virtualNode));
                    }
                }

            });
            return virtualNodeMap;
        }

        public int[,] CreateDependencyMatrix()
        {

            this.nodeSet.SetNodeMap(handlingVirtualNode(this.dependencyList));
            /*
             * number of rule is not always matched with the last ruleId in Node 
             */
            int numberOfRules = Node<T>.GetStaticNodeId();
            
            int[,] dependencyMatrix = new int[numberOfRules, numberOfRules];
        
            
            this.dependencyList.ForEach(dp => {
                int parentId = dp.GetParentNode().GetNodeId();
                int childId = dp.GetChildNode().GetNodeId();
                int dpType = dp.GetDependencyType();
                dependencyMatrix[parentId, childId] = dpType;
            });
            
            return dependencyMatrix;
    
        }

       
        public void SetNodeSet(NodeSet<T> ns)
        {
            this.nodeSet = ns;
        }

        private int HandleNotKnownManOptPos(string firstTokenString, int dependencyType)
        {
            if (dependencyType != 0)
            {
                if (firstTokenString.Contains("NOT"))
                {
                    dependencyType |= DependencyType.GetNot();
                }
                if (firstTokenString.Contains("KNOWN"))
                {
                    dependencyType |= DependencyType.GetKnown();
                }
                if (firstTokenString.Contains("MANDATORY"))
                {
                    dependencyType |= DependencyType.GetMandatory();
                }
                if (firstTokenString.Contains("OPTIONALLY"))
                {
                    dependencyType |= DependencyType.GetOptional();
                }
                if (firstTokenString.Contains("POSSIBLY"))
                {
                    dependencyType |= DependencyType.GetPossible();
                }
            }

            return dependencyType;
        }


    }
}
