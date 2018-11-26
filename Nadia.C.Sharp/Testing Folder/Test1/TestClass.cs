using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.InferenceEngineFolder;
using Nadia.C.Sharp.NodeFolder;
using Nadia.C.Sharp.RuleParserFolder;
using Nadia.C.Sharp.TestingFolder;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Nadia.C.Sharp
{
    public class TestClass
    {
        public static void Testing_for_ALL_Node_Lines_and_features_10()
        {
            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
            string newFilePath = filePath + @"/Testing Folder/Test1/Testing for ALL Node Lines and features.txt";

            RuleSetReader ilr = new RuleSetReader();
            ilr.SetFileSource(newFilePath);
            RuleSetParser isf = new RuleSetParser();
            RuleSetScanner rsc = new RuleSetScanner(ilr, isf);
            rsc.ScanRuleSet();
            rsc.EstablishNodeSet(null);

            StreamReader streamReader = new StreamReader(filePath + @"/Testing Folder/Test1/Comparison Testing for ALL Node Lines and features.txt");
            string line;
            List<string> nodeListMock = new List<string>();
            string[] tempArray;
            Dictionary<string, NodeObject_For_Inference_Test> nameMap = new Dictionary<string, NodeObject_For_Inference_Test>();


            while ((line = streamReader.ReadLine()) != null)
            {
                tempArray = Regex.Split(line, "&");
                nodeListMock.Add(tempArray[0].Trim());
                if(line.Length != 0)
                {
                    NodeObject_For_Inference_Test nfit = new NodeObject_For_Inference_Test(tempArray[0].Trim(), Regex.Split(tempArray[1].Trim(), ":"));
                    nameMap.Add(tempArray[0].Trim(), nfit);
                }

            }
            streamReader.Close();

            List<int> comparisonTempList = new List<int>();
            Enumerable.Range(0, nodeListMock.Count).ToList().ForEach(t => {
                string mockNode = nodeListMock[t];
                Node actualNode = isf.GetNodeSet().GetNodeSortedList()[t];
                if (actualNode.GetNodeName().Equals("person's nationality IS \"Australian\"")
                    && mockNode.Equals(actualNode.GetNodeName())
                    && actualNode.GetVariableName().Equals("person's nationality")
                    && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                    && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("Australian"))
                {
                    comparisonTempList.Add(t);

                }
                else if (actualNode.GetNodeName().Equals("person's name = \"troy jones\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's name")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("troy jones"))
                {
                    comparisonTempList.Add(t);

                }
                else if (actualNode.GetNodeName().Equals("person's dob > 01/01/1990")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person's dob")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DATE)
                         && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("1990/01/01"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's name IS IN LIST: name list")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's name")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("name list"))

                {
                    comparisonTempList.Add(t);

                }
                else if (actualNode.GetNodeName().Equals("person passport type = \"Australian\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person passport type")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("Australian"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person passport issued country = \"Australian\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person passport issued country")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("Australian"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person age >18")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person age")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.INTEGER)
                        && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("18"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("a number of countries the person has travelled so far >= 40")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("a number of countries the person has travelled so far")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.INTEGER)
                        && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("40"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("current location of person's passport = the place the person normally locate the passport")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("current location of person's passport")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.STRING)
                        && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("the place the person normally locate the passport"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's citizenship status = \"canceled\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person's citizenship status")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("canceled"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's legally yearly period of stay in Australia <= 20")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person's legally yearly period of stay in Australia")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.INTEGER)
                        && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("20"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's permanent residentship status = \"canceled\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person's permanent residentship status")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("canceled"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's drinking habit = \"frequent drinker\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person's drinking habit")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("frequent drinker"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's account IS IN LIST: premium account type")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's account")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("premium account type"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's drinking habit IS \"social drinker\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's drinking habit")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("social drinker"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's drinking habit IS \"frequent drinker\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's drinking habit")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("frequent drinker"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's drinking habit IS \"heavy drinker\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's drinking habit")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("heavy drinker"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("number of drinks the person consumes a week IS CALC ( number of drinks the person consumes an hour * hours of drinks a day * (5-1))")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("number of drinks the person consumes a week")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("( number of drinks the person consumes an hour * hours of drinks a day * (5-1))"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("ALL service ITERATE: LIST OF service history")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetLineType().Equals(LineType.ITERATE)
                        && ((IterateLine)actualNode).GetNumberOfTarget().Equals("ALL")
                        && ((IterateLine)actualNode).GetGivenListName().Equals("service history")
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals(FactValueType.STRING)
                        )
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("enlistment date >= 01/07/1951")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetLineType().Equals(LineType.COMPARISON)
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DATE)
                        )
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("discharge date <= 6/12/1972")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetLineType().Equals(LineType.COMPARISON)
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DATE)
                        )
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("service type IS IN LIST: Special service")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetLineType().Equals(LineType.VALUE_CONCLUSION)
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DATE)
                        )
                {
                    comparisonTempList.Add(t);
                }
                else if (mockNode.Equals(actualNode.GetNodeName()))
                {
                    comparisonTempList.Add(t);
                }
            });

            if(comparisonTempList.Count ==  nodeListMock.Count)
            {
                Console.WriteLine("Node side has been tested and passed");
            }
        
            InferenceEngine ie = new InferenceEngine(isf.GetNodeSet());
            Assessment ass = new Assessment(isf.GetNodeSet(), isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName());
            int i = 0;

            FactValue goalRuleValue = null;
            ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue);

            while (goalRuleValue == null || !ie.GetAssessmentState().AllMandatoryNodeDetermined())
            {

                Node nextQuestionNode = ie.GetNextQuestion(ass);
                Dictionary<string, FactValueType> questionFvtMap = ie.FindTypeOfElementToBeAsked(nextQuestionNode);

    //          FactValueType fvt = null;
                string answer = "";
            
                foreach(string question in ie.GetQuestionsFromNodeToBeAsked(nextQuestionNode))
                {
                    Console.WriteLine("questionFvt :"+questionFvtMap[question]);
                    Console.WriteLine("Question: " + question+"?");
                
                    if(question.Equals("1st service discharge type"))
                    {
                        answer = "Medical";
                    }
                    else if(question.Equals("2nd service discharge type"))
                    {
                        answer = "Dishonor";
                    }
                    else if(question.Equals("3rd service discharge type"))
                    {
                        answer = "Voluntary";
                    }
                    else if(question.Equals("3rd service service type"))
                    {
                        answer = "Turongga";
                    }
                    else if(question.Equals("2nd service service type"))
                    {
                        answer = "Angular force";
                    }
                    else if(question.Equals("1st service service type"))
                    {
                        answer = "SAP";
                    }
                    else if(question.Equals("3rd service discharge date"))
                    {
                        answer = "05/12/1993";
                    }
                    else if(question.Equals("2nd service discharge date"))
                    {
                        answer = "05/12/2003";
                    }
                    else if(question.Equals("1st service discharge date"))
                    {
                        answer = "05/12/1970";
                    }
                    else if(question.Equals("3rd service enlistment date"))
                    {
                        answer = "02/07/1995";
                    }
                    else if(question.Equals("2nd service enlistment date"))
                    {
                        answer = "02/07/1995";
                    }
                    else if(question.Equals("1st service enlistment date"))
                    {
                        answer = "2/07/1955";
                    }
                    else if(question.Equals("number of services"))
                    {
                        answer = "3";
                    }
                    else if(question.Equals("person's name"))
                    {
                        answer = "John Smith";
                    }
                    else if(question.Equals("one of person's parents is Australian"))
                    {
                        answer = "true";
                    }
                    else if(question.Equals("person's dob"))
                    {
                        answer = "11/12/1994";
                    }
                    else if(question.Equals("the person missed the flight"))
                    {
                        answer = "false";
                    }
                    else if(question.Equals("number of drinks the person consumes an hour"))
                    {
                        answer = "10";
                    }
                    else if(question.Equals("hours of drinks a day"))
                    {
                        answer = "9";
                    }
                    else if(question.Equals("person's account"))
                    {
                        answer = "HAHAHOHO";
                    }
                    else if(i == 0)
                    {
                        answer = "true";
                    }
                    else if(question.Equals("person passport type"))
                    {
                        answer = "Australian";
                    }
                    else if(question.ToLower().Equals("person passport issued country"))
                    {
                        answer = "Australia";
                    }
                    else if(question.ToLower().Equals("person age"))
                    {
                        answer = "18";
                    }
                    else if(question.ToLower().Equals("a number of countries the person has travelled so far"))
                    {
                        answer = "40";
                    }
                    else if(question.ToLower().Equals("current location of person's passport"))
                    {
                        answer = "there";
                    }
                    else if(question.ToLower().Equals("the place the person normally locate the passport"))
                    {
                        answer = "here";
                    }
                    else if(question.ToLower().Equals("person's passport is in a police station"))
                    {
                        answer = "false";
                    }
                    else if(question.ToLower().Equals("person's citizenship status"))
                    {
                        answer = "canceled";
                    }
                    else if(question.ToLower().Equals("person's legally yearly period of stay in australia"))
                    {
                        answer = "19";
                    }
                    else if(question.ToLower().Equals("person's permanent residentship status"))
                    {
                        answer = "resident";
                    }
    //              else if(question.Equals("person's dob"))
    //              {
    //                  answer = "false";
    //              }
                    else if(question.Equals("the person was born in Australia"))
                    {
                        answer = "true";
                    }
                    else if(i< 3)
                    {
                        answer = "true";
                    }
                    else
                    {
                        answer = nameMap[question].GetValue();
                    }
                    Console.WriteLine("Answer: "+answer);

                    ie.FeedAnswerToNode(nextQuestionNode, question, FactValue.GenerateFactValue(questionFvtMap[question], answer), ass);
                    i++;

                    ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue);

                }

            }

            Dictionary<string, FactValue> workingMemory = ie.GetAssessmentState().GetWorkingMemory();


            ie.GenerateSortedSummaryList().ForEach(node =>{
                if (workingMemory[node].GetFactValueType().Equals(FactValueType.LIST))
                {
                    ((FactListValue)workingMemory[node]).GetValue().ForEach(item => Console.WriteLine("List Value - " + node + " : " + item));
                }
                {
                    Console.WriteLine(node + " : " + FactValue.GetValueInString(workingMemory[node].GetFactValueType(), workingMemory[node]));
                }
            }); 
    
        }


        public static void Testing_Whole_Features_Of_ValueConclusionLine_ComparisonLine_and_ExprConclusionLine_9()
        {

            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
            string newFilePath = filePath + @"/Testing Folder/Test1/Testing for whole features of ValueConclusionLine, ComparisonLine and ExprConclusionLine.txt";

            RuleSetReader ilr = new RuleSetReader();
            ilr.SetFileSource(newFilePath);
            RuleSetParser isf = new RuleSetParser();
            RuleSetScanner rsc = new RuleSetScanner(ilr, isf);
            rsc.ScanRuleSet();
            rsc.EstablishNodeSet(null);

            StreamReader streamReader = new StreamReader(filePath + @"/Testing Folder/Test1/Comparison for testing whole features of ValueConclusionLine, ComparisonLine and ExprConclusionLine.txt");
            string line;
            List<string> nodeListMock = new List<string>();
            string[] tempArray;
            Dictionary<string, NodeObject_For_Inference_Test> nameMap = new Dictionary<string, NodeObject_For_Inference_Test>();


            while((line = streamReader.ReadLine()) != null)
            {

                tempArray = Regex.Split(line, "&");
                nodeListMock.Add(tempArray[0].Trim());

                NodeObject_For_Inference_Test nfit = new NodeObject_For_Inference_Test(tempArray[0].Trim(), Regex.Split(tempArray[1].Trim(), ":"));
                nameMap.Add(tempArray[0].Trim(), nfit);

            }
            streamReader.Close();

            List<int> comparisonTempList = new List<int>();
            Enumerable.Range(0, nodeListMock.Count).ToList().ForEach(t =>
            {
                string mockNode = nodeListMock[t];
                Node actualNode = isf.GetNodeSet().GetNodeSortedList()[t];

                if (actualNode.GetNodeName().Equals("person's nationality IS \"Australian\"")
                    && mockNode.Equals(actualNode.GetNodeName())
                    && actualNode.GetVariableName().Equals("person's nationality")
                    && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                    && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("Australian"))
                {
                    comparisonTempList.Add(t);

                }
                else if (actualNode.GetNodeName().Equals("person's name = \"troy jones\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's name")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("troy jones"))
                {
                    comparisonTempList.Add(t);

                }
                else if (actualNode.GetNodeName().Equals("person's dob > 01/01/1990")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person's dob")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DATE)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("1990/01/01"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's name IS IN LIST: name list")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's name")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("name list"))

                {
                    comparisonTempList.Add(t);

                }
                else if (actualNode.GetNodeName().Equals("person passport type = \"Australian\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person passport type")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("Australian"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person passport issued country = \"Australian\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person passport issued country")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("Australian"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person age >18")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person age")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.INTEGER)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("18"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("a number of countries the person has travelled so far >= 40")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("a number of countries the person has travelled so far")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.INTEGER)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("40"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("current location of person's passport = the place the person normally locate the passport")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("current location of person's passport")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("the place the person normally locate the passport"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's citizenship status = \"canceled\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person's citizenship status")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("canceled"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's legally yearly period of stay in Australia <= 20")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person's legally yearly period of stay in Australia")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.INTEGER)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("20"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's permanent residentship status = \"canceled\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person's permanent residentship status")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("canceled"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's drinking habit = \"frequent drinker\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && ((ComparisonLine)actualNode).GetLHS().Equals("person's drinking habit")
                        && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("frequent drinker"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's account IS IN LIST: premium account type")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's account")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("premium account type"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's drinking habit IS \"social drinker\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's drinking habit")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("social drinker"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's drinking habit IS \"frequent drinker\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's drinking habit")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("frequent drinker"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person's drinking habit IS \"heavy drinker\"")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("person's drinking habit")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("heavy drinker"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("number of drinks the person consumes a week IS CALC ( number of drinks the person consumes an hour * hours of drinks a day * (5-1))")
                        && mockNode.Equals(actualNode.GetNodeName())
                        && actualNode.GetVariableName().Equals("number of drinks the person consumes a week")
                        && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.STRING)
                        && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("( number of drinks the person consumes an hour * hours of drinks a day * (5-1))"))
                {
                    comparisonTempList.Add(t);
                }
                else if (mockNode.Equals(actualNode.GetNodeName()))
                {
                    comparisonTempList.Add(t);
                }
            });

            if(comparisonTempList.Count ==  nodeListMock.Count)
            {
                Console.WriteLine("Node side has been tested and passed");
            }
            
            InferenceEngine ie = new InferenceEngine(isf.GetNodeSet());
            Assessment ass = new Assessment(isf.GetNodeSet(), isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName());
            ie.SetAssessment(ass);
            int i = 0;
            FactValue goalRuleValue = null;
            ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue);

            while (goalRuleValue == null || !ie.GetAssessmentState().AllMandatoryNodeDetermined())
            {
                
                Node nextQuestionNode = ie.GetNextQuestion(ie.GetAssessment());
                Dictionary<string, FactValueType> questionFvtMap = ie.FindTypeOfElementToBeAsked(nextQuestionNode);

                string answer = "";
                            
                foreach(string question in ie.GetQuestionsFromNodeToBeAsked(nextQuestionNode))
                {
                    Console.WriteLine("questionFvt :"+questionFvtMap[question]);
                    Console.WriteLine("Question: " + question+"?");
                
                    if(question.Equals("person's name"))
                    {
                        answer = "John Smith";
                    }
                    else if(question.Equals("one of person's parents is Australian"))
                    {
                        answer = "true";
                    }
                    else if(question.Equals("person's dob"))
                    {
                        answer = "11/12/1994";
                    }
                    else if(question.Equals("the person missed the flight"))
                    {
                        answer = "false";
                    }
                    else if(question.Equals("number of drinks the person consumes an hour"))
                    {
                        answer = "10";
                    }
                    else if(question.Equals("hours of drinks a day"))
                    {
                        answer = "9";
                    }
                    else if(question.Equals("person's account"))
                    {
                        answer = "HAHAHOHO";
                    }
                    else if(i == 0)
                    {
                        answer = "true";
                    }
                    else if(question.Equals("person passport type"))
                    {
                        answer = "Australian";
                    }
                    else if(question.ToLower().Equals("person passport issued country"))
                    {
                        answer = "Australia";
                    }
                    else if(question.ToLower().Equals("person age"))
                    {
                        answer = "18";
                    }
                    else if(question.ToLower().Equals("a number of countries the person has travelled so far"))
                    {
                        answer = "40";
                    }
                    else if(question.ToLower().Equals("current location of person's passport"))
                    {
                        answer = "there";
                    }
                    else if(question.ToLower().Equals("the place the person normally locate the passport"))
                    {
                        answer = "here";
                    }
                    else if(question.ToLower().Equals("person's passport is in a police station"))
                    {
                        answer = "false";
                    }
                    else if(question.ToLower().Equals("person's citizenship status"))
                    {
                        answer = "canceled";
                    }
                    else if(question.ToLower().Equals("person's legally yearly period of stay in australia"))
                    {
                        answer = "19";
                    }
                    else if(question.ToLower().Equals("person's permanent residentship status"))
                    {
                        answer = "resident";
                    }
    //              else if(question.Equals("person's dob"))
    //              {
    //                  answer = "false";
    //              }
                    else if(question.Equals("the person was born in Australia"))
                    {
                        answer = "true";
                    }
                    else if(i< 3)
                    {
                        answer = "true";
                    }
                    else
                    {
                        answer = nameMap[question].GetValue();
                    }

                    Console.WriteLine("Answer: "+answer);


                    ie.FeedAnswerToNode(nextQuestionNode, question, FactValue.GenerateFactValue(questionFvtMap[question], answer), ass);
                    i++;

                    ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue);

                }
            }

            Dictionary<string, FactValue> workingMemory = ie.GetAssessmentState().GetWorkingMemory();
            ie.GetAssessmentState().GetSummaryList().ForEach(node =>{
                if (workingMemory[node].GetFactValueType().Equals(FactValueType.LIST))
                {
                    ((FactListValue)workingMemory[node]).GetValue().ForEach(item=>Console.WriteLine(node + " : " + item));
                }
                else
                {
                    Console.WriteLine(node + " : " + FactValue.GetValueInString(workingMemory[node].GetFactValueType(), workingMemory[node]));
                }
            }); 

    
        }

        public static void Testing_Whole_Features_Of_ValueConclusionLIne_and_ComparisonLine_8()
        {
            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
            string newFilePath = filePath + @"/Testing Folder/Test1/Testing for whole features of ValueConclusionLine and ComparisonLine.txt";

            RuleSetReader ilr = new RuleSetReader();
            ilr.SetFileSource(newFilePath);
            RuleSetParser isf = new RuleSetParser();
            RuleSetScanner rsc = new RuleSetScanner(ilr, isf);
            rsc.ScanRuleSet();
            rsc.EstablishNodeSet(null);

            StreamReader streamReader = new StreamReader(filePath + @"/Testing Folder/Test1/Comparison Testing for Whole features of ValueConclusionLine and ComparisonLine.txt");
            string line;
            List<string> nodeListMock = new List<string>();
            string[] tempArray;
            Dictionary<string, NodeObject_For_Inference_Test> nameMap = new Dictionary<string, NodeObject_For_Inference_Test>();

            while((line = streamReader.ReadLine()) != null)
            {

                tempArray = Regex.Split(line, "-");
                nodeListMock.Add(tempArray[0].Trim());
                Console.WriteLine("line: " + line);

                NodeObject_For_Inference_Test nfit = new NodeObject_For_Inference_Test(tempArray[0].Trim(), Regex.Split(tempArray[1].Trim(), ":"));
                nameMap.Add(tempArray[0].Trim(), nfit);
            }
            streamReader.Close();

            List<int> comparisonTempList = new List<int>();
            Enumerable.Range(0, nodeListMock.Count()).ToList().ForEach((t)=>{
                string mockNode = nodeListMock[t];
                Node actualNode = isf.GetNodeSet().GetNodeSortedList()[t];
                if (actualNode.GetNodeName().Equals("person's nationality IS “Australian”")
                    && mockNode.Equals(actualNode.GetNodeName())
                    && actualNode.GetVariableName().Equals("person's nationality")
                    && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                    && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("Australian"))
                {
                    comparisonTempList.Add(t);

                }
                else if (actualNode.GetNodeName().Equals("person's name IS IN LIST: name list")
                         && mockNode.Equals(actualNode.GetNodeName())
                         && actualNode.GetVariableName().Equals("person's name")
                         && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.STRING)
                         && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("name list"))

                {
                    comparisonTempList.Add(t);

                }
                else if (actualNode.GetNodeName().Equals("person passport type = “Australian”")
                         && mockNode.Equals(actualNode.GetNodeName())
                         && ((ComparisonLine)actualNode).GetLHS().Equals("person passport type")
                         && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                         && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("Australian"))
                {
                        comparisonTempList.Add(t);
                }
                    else if (actualNode.GetNodeName().Equals("person passport issued country = “Australian”")
                             && mockNode.Equals(actualNode.GetNodeName())
                             && ((ComparisonLine)actualNode).GetLHS().Equals("person passport issued country")
                             && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                             && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("Australian"))
                {
                        comparisonTempList.Add(t);
                }
                    else if (actualNode.GetNodeName().Equals("person age >18")
                             && mockNode.Equals(actualNode.GetNodeName())
                             && ((ComparisonLine)actualNode).GetLHS().Equals("person age")
                             && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.INTEGER)
                             && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("18"))
                {
                        comparisonTempList.Add(t);
                }
                    else if (actualNode.GetNodeName().Equals("a number of countries the person has travelled so far >= 40")
                             && mockNode.Equals(actualNode.GetNodeName())
                             && ((ComparisonLine)actualNode).GetLHS().Equals("a number of countries the person has travelled so far")
                             && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.INTEGER)
                             && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("40"))
                {
                        comparisonTempList.Add(t);
                }
                    else if (actualNode.GetNodeName().Equals("current location of person's passport = the place the person normally locate the passport")
                             && mockNode.Equals(actualNode.GetNodeName())
                             && ((ComparisonLine)actualNode).GetLHS().Equals("current location of person's passport")
                             && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.STRING)
                             && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("the place the person normally locate the passport"))
                {
                        comparisonTempList.Add(t);
                }
                    else if (actualNode.GetNodeName().Equals("person's citizenship status = “canceled”")
                             && mockNode.Equals(actualNode.GetNodeName())
                             && ((ComparisonLine)actualNode).GetLHS().Equals("person's citizenship status")
                             && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.STRING)
                             && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("canceled"))
                {
                        comparisonTempList.Add(t);
                }
                    else if (actualNode.GetNodeName().Equals("person's legally yearly period of stay in Australia <= 20")
                             && mockNode.Equals(actualNode.GetNodeName())
                             && ((ComparisonLine)actualNode).GetLHS().Equals("person's legally yearly period of stay in Australia")
                             && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.INTEGER)
                             && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("20"))
                {
                        comparisonTempList.Add(t);
                }
                    else if (actualNode.GetNodeName().Equals("person's permanent residentship status = “canceled”")
                             && mockNode.Equals(actualNode.GetNodeName())
                             && ((ComparisonLine)actualNode).GetLHS().Equals("person's permanent residentship status")
                             && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.STRING)
                             && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("canceled"))
                {
                        comparisonTempList.Add(t);
                }
                    else if (mockNode.Equals(actualNode.GetNodeName()))
                {
                        comparisonTempList.Add(t);
                }
            });
            if(comparisonTempList.Count ==  nodeListMock.Count)
            {
                Console.WriteLine("Node side has been tested and passed");
            }



                
            InferenceEngine ie = new InferenceEngine(isf.GetNodeSet());
            Assessment ass = new Assessment(isf.GetNodeSet(), isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName());

            int i = 0;
            FactValue goalRuleValue = null;
            ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue);
            while (goalRuleValue == null)
            {
            
                Node nextQuestionNode = ie.GetNextQuestion(ass);
                Dictionary<string, FactValueType> questionFvtMap = ie.FindTypeOfElementToBeAsked(nextQuestionNode);


                string answer;
            
                foreach(string question in ie.GetQuestionsFromNodeToBeAsked(nextQuestionNode))
                {
                    Console.WriteLine("questionFvt :"+questionFvtMap[question]);
                    Console.WriteLine("Question: " + question+"?");
                   
                    if(question.Equals("person's name"))
                    {
                        answer = "John Smith";
                    }
                    else if(question.Equals("person's dob"))
                    {
                        answer = "11/12/1934";
                    }
                    else if(question.Equals("the person missed the flight"))
                    {
                        answer = "false";
                    }
                    else if(i == 0)
                    {
                        answer = "true";
                    }
                    else if(question.Equals("person passport type"))
                    {
                        answer = "Australian";
                    }
                    else if(question.ToLower().Equals("person passport issued country"))
                    {
                        answer = "Australia";
                    }
                    else if(question.ToLower().Equals("person age"))
                    {
                        answer = "18";
                    }
                    else if(question.ToLower().Equals("a number of countries the person has travelled so far"))
                    {
                        answer = "40";
                    }
                    else if(question.ToLower().Equals("current location of person's passport"))
                    {
                        answer = "there";
                    }
                    else if(question.ToLower().Equals("the place the person normally locate the passport"))
                    {
                        answer = "here";
                    }
                    else if(question.ToLower().Equals("person's passport is in a police station"))
                    {
                        answer = "false";
                    }
                    else if(question.ToLower().Equals("person's citizenship status"))
                    {
                        answer = "canceled";
                    }
                    else if(question.ToLower().Equals("person's legally yearly period of stay in australia"))
                    {
                        answer = "19";
                    }
                    else if(question.ToLower().Equals("person's permanent residentship status"))
                    {
                        answer = "resident";
                    }
    //              else if(question.Equals("person's dob"))
    //              {
    //                  answer = "false";
    //              }
    //              else if(question.Equals("the person was born in Australia"))
    //              {
    //                  answer = "false";
    //              }
                    else if(i< 3)
                    {
                        answer = "true";
                    }
                    else
                    {
                        answer = nameMap[question].GetValue();
                    }
                    Console.WriteLine("Answer: "+answer);

                    ie.FeedAnswerToNode(nextQuestionNode, question, FactValue.GenerateFactValue(questionFvtMap[question], answer), ass);
                    i++;

                    ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue);
                
                }

                
            }

            Dictionary<string, FactValue> workingMemory = ie.GetAssessmentState().GetWorkingMemory();
            ie.GetAssessmentState().GetSummaryList().ToList().ForEach(node =>{
                Console.WriteLine(node + " : " + FactValue.GetValueInString(workingMemory[node].GetFactValueType(), workingMemory[node]));
            }); 
        
        }


        public static void Tesing_Full_ValueConclusion_Comparison_7()
        {
            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
            string newFilePath = filePath + @"/Testing Folder/Test1/Testing full ValueConclusion and Comparison.txt";

            RuleSetReader ilr = new RuleSetReader();
            ilr.SetFileSource(newFilePath);

            RuleSetParser isf = new RuleSetParser();
            RuleSetScanner rsc = new RuleSetScanner(ilr, isf);
            rsc.ScanRuleSet();
            rsc.EstablishNodeSet(null);

            StreamReader streamReader = new StreamReader(filePath+@"/Testing Folder/Test1/Comparison for Testing full ValueConclusion and Comparison.txt");

            string line;
            List<string> nodeListMock = new List<string>();
            string[] tempArray;
            Dictionary<string, NodeObject_For_Inference_Test> nameMap = new Dictionary<string, NodeObject_For_Inference_Test>();

            while((line = streamReader.ReadLine()) != null)
            {

                tempArray = Regex.Split(line, "-");
                nodeListMock.Add(tempArray[0].Trim());
                Console.WriteLine("line: "+line);

                NodeObject_For_Inference_Test nfit = new NodeObject_For_Inference_Test(tempArray[0].Trim(), Regex.Split(tempArray[1].Trim(), ":"));
                nameMap.Add(tempArray[0].Trim(), nfit);
            }
            streamReader.Close();

            List<int> comparisonTempList = new List<int>();
            Enumerable.Range(0, nodeListMock.Count).ToList().ForEach((t) =>{
                string mockNode = nodeListMock[t];
                Node actualNode = isf.GetNodeSet().GetNodeSortedList()[t];
                if (actualNode.GetNodeName().Equals("person's nationality IS “Australian”")
                    && mockNode.Equals(actualNode.GetNodeName())
                    && actualNode.GetVariableName().Equals("person's nationality")
                    && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                    && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("Australian"))
                {
                    comparisonTempList.Add(t);

                }
                else if (actualNode.GetNodeName().Equals("person's name IS IN LIST: name list")
                         && mockNode.Equals(actualNode.GetNodeName())
                         && actualNode.GetVariableName().Equals("person's name")
                         && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.STRING)
                         && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("name list"))

                {
                    comparisonTempList.Add(t);

                }
                else if (actualNode.GetNodeName().Equals("person passport type = “Australian”")
                         && mockNode.Equals(actualNode.GetNodeName())
                         && ((ComparisonLine)actualNode).GetLHS().Equals("person passport type")
                         && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                         && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("Australian"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person passport issued country = “Australian”")
                         && mockNode.Equals(actualNode.GetNodeName())
                         && ((ComparisonLine)actualNode).GetLHS().Equals("person passport issued country")
                         && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                         && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("Australian"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("person age >18")
                         && mockNode.Equals(actualNode.GetNodeName())
                         && ((ComparisonLine)actualNode).GetLHS().Equals("person age")
                         && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.INTEGER)
                         && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("18"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("a number of countries the person has travelled so far >= 40")
                         && mockNode.Equals(actualNode.GetNodeName())
                         && ((ComparisonLine)actualNode).GetLHS().Equals("a number of countries the person has travelled so far")
                         && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.INTEGER)
                         && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("40"))
                {
                    comparisonTempList.Add(t);
                }
                else if (actualNode.GetNodeName().Equals("current location of person's passport = the place the person normally locate the passport")
                         && mockNode.Equals(actualNode.GetNodeName())
                         && ((ComparisonLine)actualNode).GetLHS().Equals("current location of person's passport")
                         && ((ComparisonLine)actualNode).GetRHS().GetFactValueType().Equals(FactValueType.STRING)
                         && FactValue.GetValueInString(((ComparisonLine)actualNode).GetRHS().GetFactValueType(), ((ComparisonLine)actualNode).GetRHS()).Equals("the place the person normally locate the passport"))
                {
                    comparisonTempList.Add(t);
                }
                else if (mockNode.Equals(actualNode.GetNodeName()))
                {
                    comparisonTempList.Add(t);
                }
            });
            if(comparisonTempList.Count ==  nodeListMock.Count())
            {
                Console.WriteLine("Node side has been tested and passed");
            }

            InferenceEngine ie = new InferenceEngine(isf.GetNodeSet());
            Assessment ass = new Assessment(isf.GetNodeSet(), isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName());
            int i = 0;

            FactValue goalRuleValue = null;
            ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue);
            while (goalRuleValue == null)
            {
                
                Node nextQuestionNode = ie.GetNextQuestion(ass);
                Dictionary<string, FactValueType> questionFvtMap = ie.FindTypeOfElementToBeAsked(nextQuestionNode);

                string answer;
                
                foreach(string question in ie.GetQuestionsFromNodeToBeAsked(nextQuestionNode))
                {
                    Console.WriteLine("questionFvt :"+questionFvtMap[question]);
                    Console.WriteLine("Question: " + question+"?");

                    if(question.Equals("person's name"))
                    {
                        answer = "John Smith";
                    }
                    else if(question.Equals("person's dob"))
                    {
                        answer = "11/12/1934";
                    }
                        else if(question.Equals("the person missed the flight"))
                    {
                        answer = "false";
                    }
                    else if(i == 0)
                    {
                        answer = "true";
                    }
                    else if(question.Equals("person passport type"))
                    {
                        answer = "Australian";
                    }
                    else if(question.ToLower().Equals("person passport issued country"))
                    {
                        answer = "Australia";
                    }
                    else if(question.ToLower().Equals("person age"))
                    {
                        answer = "19";
                    }
                    else if(question.ToLower().Equals("a number of countries the person has travelled so far"))
                    {
                        answer = "40";
                    }
                    else if(question.ToLower().Equals("current location of person's passport"))
                    {
                        answer = "there";
                    }
                    else if(question.ToLower().Equals("the place the person normally locate the passport"))
                    {
                        answer = "here";
                    }
                    else if(question.ToLower().Equals("person's passport is in a police station"))
                    {
                        answer = "false";
                    }
    //              else if(question.Equals("person's dob"))
    //              {
    //                  answer = "false";
    //              }
    //              else if(question.Equals("the person was born in Australia"))
    //              {
    //                  answer = "false";
    //              }
                    else if(i< 3)
                    {
                        answer = "true";
                    }
                    else
                    {
                        answer = nameMap[question].GetValue();
                    }
                    Console.WriteLine("Answer: "+answer);

                    ie.FeedAnswerToNode(nextQuestionNode, question, FactValue.GenerateFactValue(questionFvtMap[question], answer), ass);
                    i++;

                    ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue);
                }

            }

            Dictionary<string, FactValue> workingMemory = ie.GetAssessmentState().GetWorkingMemory();
            ie.GetAssessmentState().GetSummaryList().ForEach(node =>{
                Console.WriteLine(node + " : " + FactValue.GetValueInString(workingMemory[node].GetFactValueType(), workingMemory[node]));
            }); 
    
        }

        public static void Testing_ValueConclusionLine_6()
        {

            RuleSetReader ilr = new RuleSetReader();


            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
            string newFilePath = filePath + @"/Testing Folder/Test1/Testing ValueConclusionLine with NOT, KNOW, IS, and IS IN LIST features.txt";

            ilr.SetFileSource(newFilePath);

            RuleSetParser isf = new RuleSetParser();
            RuleSetScanner rsc = new RuleSetScanner(ilr, isf);
            rsc.ScanRuleSet();
            rsc.EstablishNodeSet(null);

            StreamReader streamReader = new StreamReader(filePath+@"/Testing Folder/Test1/Testing ValueConclusionLine Comparison.txt");

            string line;
            List<string> nodeListMock = new List<string>();
            string[] tempArray;
            Dictionary<string, NodeObject_For_Inference_Test> nameMap = new Dictionary<string, NodeObject_For_Inference_Test>();

            while((line = streamReader.ReadLine()) != null)
            {

                tempArray = Regex.Split(line, "-");
                nodeListMock.Add(tempArray[0].Trim());
                Console.WriteLine("line: "+line);

                NodeObject_For_Inference_Test nfit = new NodeObject_For_Inference_Test(tempArray[0].Trim(), Regex.Split(tempArray[1].Trim(), ":"));
                nameMap.Add(tempArray[0].Trim(), nfit);
            }
            streamReader.Close();

            List<int> comparisonTempList = new List<int>();
            Enumerable.Range(0, nodeListMock.Count).ToList().ForEach(t =>
            {
                string mockNode = nodeListMock[t];
                Node actualNode = isf.GetNodeSet().GetNodeSortedList()[t];

                if (actualNode.GetNodeName().Equals("person's nationality IS “Australian”")
                    && mockNode.Equals(actualNode.GetNodeName())
                    && actualNode.GetVariableName().Equals("person's nationality")
                    && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.DEFI_STRING)
                    && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(), actualNode.GetFactValue()).Equals("Australian")) 
                {
                    comparisonTempList.Add(t);

                }
                else if (actualNode.GetNodeName().Equals("person's name IS IN LIST: name list")
                         && mockNode.Equals(actualNode.GetNodeName())
                         && actualNode.GetVariableName().Equals("person's name")
                         && actualNode.GetFactValue().GetFactValueType().Equals(FactValueType.STRING)
                         && FactValue.GetValueInString(actualNode.GetFactValue().GetFactValueType(),actualNode.GetFactValue()).Equals("name list"))

                {
                    comparisonTempList.Add(t);

                }
                else if (mockNode.Equals(actualNode.GetNodeName()))
                {
                    comparisonTempList.Add(t);
                }
            });
            if(comparisonTempList.Count ==  nodeListMock.Count)
            {
                Console.WriteLine("Node side has been tested and passed");
            }

            InferenceEngine ie = new InferenceEngine(isf.GetNodeSet());
            Assessment ass = new Assessment(isf.GetNodeSet(), isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName());
            int i = 0;
            FactValue goalRuleValue = null;
            ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue);
            while (goalRuleValue == null)
            {
                
                Node nextQuestionNode = ie.GetNextQuestion(ass);
                Dictionary<string, FactValueType> questionFvtMap = ie.FindTypeOfElementToBeAsked(nextQuestionNode);

        
                string answer;
                
                foreach(string question in ie.GetQuestionsFromNodeToBeAsked(nextQuestionNode))
                {
                    Console.WriteLine("questionFvt :"+questionFvtMap[question]);
                    Console.WriteLine("Question: " + question+"?");
                    if(question.Equals("person's name"))
                    {
                        answer = "John Smith";
                    }
                    else if(question.Equals("person's dob"))
                    {
                        answer = "11/12/1934";
                    }
                    else if(i == 0)
                    {
                        answer = "true";
                    }
    //              else if(question.Equals("person's dob"))
    //              {
    //                  answer = "false";
    //              }
    //              else if(question.Equals("the person was born in Australia"))
    //              {
    //                  answer = "false";
    //              }
                    else if(i< 3)
                    {
                        answer = "true";
                    }
                    else
                    {
                        answer = nameMap[question].GetValue();
                    }
                    Console.WriteLine("Answer: "+answer);


                    ie.FeedAnswerToNode(nextQuestionNode, question, FactValue.GenerateFactValue(questionFvtMap[question], answer), ass);
                    i++;

                    ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue);
                }
            }

            Dictionary<string, FactValue> workingMemory = ie.GetAssessmentState().GetWorkingMemory();
            ie.GetAssessmentState().GetSummaryList().ForEach(node =>{
                    Console.WriteLine(node + " : " + FactValue.GetValueInString(workingMemory[node].GetFactValueType(), workingMemory[node]));
            }); 
        }


        public static void Testing_Inference_For_NotKnownManOpPo_5()
        {
            
        
            Dictionary<string, NodeObject_For_Inference_Test> nameMap = new Dictionary<string, NodeObject_For_Inference_Test>();

            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
            string newFilePath = filePath + @"/Testing Folder/Test1/testing NOT, KNOWN, Mandatory, Possibly, and Optionally inference.txt";

            StreamReader streamReader = new StreamReader(newFilePath);

            string line;
            while((line = streamReader.ReadLine()) != null)
            {
                string[] lineArray = Regex.Split(line, "-");
                string[] lineArraySecondLevel = Regex.Split(lineArray[1], ":");
                NodeObject_For_Inference_Test nfit = new NodeObject_For_Inference_Test(lineArray[0], lineArraySecondLevel);
                nameMap.Add(lineArray[0], nfit);
            }
            streamReader.Close();
            

            RuleSetReader ilr = new RuleSetReader();
            ilr.SetFileSource(filePath+@"/Testing Folder/Test1/testing NOT, KNOWN, Mandatory, Possibly, and Optionally.txt");
            RuleSetParser isf = new RuleSetParser();
            RuleSetScanner rsc = new RuleSetScanner(ilr, isf);
            rsc.ScanRuleSet();
            rsc.EstablishNodeSet(null);
            
            InferenceEngine ie = new InferenceEngine(isf.GetNodeSet());
            Assessment ass = new Assessment(isf.GetNodeSet(), isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName());
            //      Scanner scan = new Scanner(System.in);
            int i = 0;
            FactValue goalRuleValue = null;
            ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue);
            while (goalRuleValue == null)
            {
                
                Node nextQuestionNode = ie.GetNextQuestion(ass);
                Dictionary<string, FactValueType> questionFvtMap = ie.FindTypeOfElementToBeAsked(nextQuestionNode);

            
                string answer;
                
                foreach(string question in ie.GetQuestionsFromNodeToBeAsked(nextQuestionNode))
                {
                    Console.WriteLine("questionFvt :"+questionFvtMap[question]);
                    Console.WriteLine("Question: " + question+"?");
                    if(i == 0)
                    {
                        answer = "false";
                    }
                    else if(question.Equals("person's dob"))
                    {
                        answer = "false";
                    }
                    else if(question.Equals("the person was born in Australia"))
                    {
                        answer = "false";
                    }
                    else if(i< 3)
                    {
                        answer = "true";
                    }
                    else
                    {
                        answer = nameMap[question].GetValue();
                    }
                    Console.WriteLine("Answer: "+answer);


                    ie.FeedAnswerToNode(nextQuestionNode, question, FactValue.GenerateFactValue(questionFvtMap[question], answer), ass);
                    i++;

                    ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue); 
                }

                
            }

            Dictionary<string, FactValue> workingMemory = ie.GetAssessmentState().GetWorkingMemory();
            ie.GetAssessmentState().GetSummaryList().ForEach((node)=>{
                Console.WriteLine(node + " : " + FactValue.GetValueInString(workingMemory[node].GetFactValueType(), workingMemory[node]));
            });

           
     
        }

        public static void Testing_For_Reading_NotKnownMandatoryPossiblyAndOptionally_4()
        {

        
            RuleSetReader ilr = new RuleSetReader();
            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
            string newFilePath = filePath + @"/Testing Folder/Test1/testing NOT, KNOWN, Mandatory, Possibly, and Optionally.txt";
            ilr.SetFileSource(newFilePath);
                
            RuleSetParser isf = new RuleSetParser();
            RuleSetScanner rsc = new RuleSetScanner(ilr, isf);
            rsc.ScanRuleSet();
            rsc.EstablishNodeSet(null);
            List<string> readLine = new List<string>();
            isf.GetNodeSet().GetNodeSortedList().ForEach((node)=>{
                readLine.Add("nodeName: " + node.GetNodeName());
                isf.GetNodeSet().GetDependencyMatrix().GetToChildDependencyList(node.GetNodeId()).ForEach((dep)=>{
                    readLine.Add("dependency type: " + isf.GetNodeSet().GetDependencyMatrix().GetDependencyType(node.GetNodeId(), isf.GetNodeSet().GetNodeMap()[isf.GetNodeSet().GetNodeIdMap()[dep]].GetNodeId()));
                    readLine.Add("dependency: " + isf.GetNodeSet().GetNodeIdMap()[dep]);
                });

            });
            
            List<string> comparisonFileRead = new List<string>();
            StreamReader streamReader = new StreamReader(filePath+@"/Testing Folder/Test1/Comparison File For Reading Not Known Man Op Pos file.txt");

            String line;
            while((line = streamReader.ReadLine()) != null)
            {
                comparisonFileRead.Add(line);
            }
            streamReader.Close();

            if(readLine.Count == comparisonFileRead.Count)
            {
                List<int> tempArray = Enumerable.Range(0, readLine.Count - 1).Where((i)=> !readLine[i].Equals(comparisonFileRead[i])).ToList();
                if(tempArray.Count == 0)
                {
                    Console.WriteLine("testing successful");
                }
                else
                {
                    Console.WriteLine("testing fail");
                }
            }
            else
            {
                Console.WriteLine("testing fail");
            }
    
        }

        public static void WeddingPlanner_Inference_Test_3()
        {
            
            
            Dictionary<string, NodeObject_For_Inference_Test> nameMap = new Dictionary<string,NodeObject_For_Inference_Test>();
            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
            string newFilePath = filePath + @"/Testing Folder/Test1/Wedding_Planner Inference Test.txt";
            StreamReader streamReader = new StreamReader(newFilePath);

            string line;
            while((line = streamReader.ReadLine()) != null)
            {
                string[] lineArray = Regex.Split(line, "-");
                string[] lineArraySecondLevel = Regex.Split(lineArray[1], ":");
                NodeObject_For_Inference_Test nfit = new NodeObject_For_Inference_Test(lineArray[0], lineArraySecondLevel);
                nameMap.Add(lineArray[0], nfit);
            }
            streamReader.Close();
            
            RuleSetReader ilr = new RuleSetReader();
            ilr.SetFileSource(filePath+@"/Testing Folder/Test1/Wedding Planner.txt");
            RuleSetParser isf = new RuleSetParser();
            RuleSetScanner rsc = new RuleSetScanner(ilr, isf);
            rsc.ScanRuleSet();
            rsc.EstablishNodeSet(null);
            InferenceEngine ie = new InferenceEngine(isf.GetNodeSet());
            Assessment ass = new Assessment(isf.GetNodeSet(), isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName());
            //      Scanner scan = new Scanner(System.in);
            int i = 0;
            FactValue goalRuleValue = null;
            ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue); 
            while(goalRuleValue ==null)
            {
                

                Node nextQuestionNode = ie.GetNextQuestion(ass);
                Dictionary<string, FactValueType> questionFvtMap = ie.FindTypeOfElementToBeAsked(nextQuestionNode);

                string answer;
                
                foreach(string question in ie.GetQuestionsFromNodeToBeAsked(nextQuestionNode))
                {
                    Console.WriteLine("questionFvt :"+questionFvtMap[question]);
                    Console.WriteLine("Question: " + question+"?");

                    if(i< 3)
                    {
                        answer = "true";
                    }
                    else
                    {
                        answer = nameMap[question].GetValue();
                    }
                    Console.WriteLine("Answer: "+answer);
        //              string answer = scan.nextLine();            

                    ie.FeedAnswerToNode(nextQuestionNode, question, FactValue.GenerateFactValue(questionFvtMap[question], answer), ass);

                    i++;
                    ie.GetAssessmentState().GetWorkingMemory().TryGetValue(isf.GetNodeSet().GetNodeSortedList()[0].GetNodeName(), out goalRuleValue); 
                }

                
            }
        //      Stream<string> keyList = ie.getAssessmentState().getWorkingMemory().keySet().stream();
        //      keyList.forEach(key -> {
        //          System.out.println(key+" : "+ie.getAssessmentState().getWorkingMemory().get(key).getValue().tostring());
        //      });
            Dictionary<string, FactValue> workingMemory = ie.GetAssessmentState().GetWorkingMemory();
            ie.GetAssessmentState().GetSummaryList().ForEach(node =>{
                Console.WriteLine(node + " : " + FactValue.GetValueInString(workingMemory[node].GetFactValueType(), workingMemory[node]));
            });
            
        //      scan.close();

        }
        public static void TopoSortingTest_2()
        {

            //this testing is to check if topological sorting is done correctly or not by comparing a sorted list of 'Wedding Planner.txt' file in RuleSetParser with 'ToposortedNodeName.txt' file

            RuleSetReader ilr = new RuleSetReader();
            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
            string newFilePath = filePath+ @"/Testing Folder/Test1/Wedding Planner.txt";

            ilr.SetFileSource(newFilePath);
            RuleSetParser isf = new RuleSetParser();
            RuleSetScanner rsc = new RuleSetScanner(ilr, isf);
            rsc.ScanRuleSet();
            rsc.EstablishNodeSet(null);
            List<string> nameList = new List<string>();
            string anotherNewFilePath = filePath+ @"/Testing Folder/Test1/ToposortedNodeName.txt";
            StreamReader streamReader = new StreamReader(anotherNewFilePath);

            string line;
            while((line = streamReader.ReadLine()) != null)
            {
                nameList.Add(line);
            }
            streamReader.Close();
            List<string[]> filteredList = new List<string[]>();
            Enumerable.Range(0, nameList.Count).ToList().ForEach(i => {
                if(!isf.GetNodeSet().GetNodeSortedList()[i].GetNodeName().Equals(nameList[i]))
                {
                    filteredList.Add(new string[] { i.ToString(), nameList[i] });
                }
            });

        
            if(filteredList.Count >0)
            {
                Enumerable.Range(0, filteredList.Count).ToList().ForEach((i) =>
                {
                    Console.WriteLine("node set: " + isf.GetNodeSet().GetNodeSortedList()[Int32.Parse(filteredList[i][0])].GetNodeName());
                    Console.WriteLine("nameList: " + nameList[Int32.Parse(filteredList[i][0])]);
                });
            }
            else
            {
                Console.WriteLine("Yep!!! it's been finished correctly");
            }
        }

        public static void Testing()
        {

            // TODO Auto-generated method stub

            //string fileName = @"Testing Folder\Test1\Tokenizer_Testing.txt";
            //string currentDirectory = Path.GetDirectoryName(fileName);
            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
            filePath += @"/Testing Folder/Test1/Tokenizer_Testing.txt";
            StreamReader streamReader = new StreamReader(filePath);


            string line;
            string textstring = "";
            int lineTracking = 0;
            Tokens tk = null;
            while ((line = streamReader.ReadLine()) != null) 
            {
                line = line.Trim();
                if(!line.Equals("") && !Regex.IsMatch(line, "^\\/.*"))
                {
                    if(lineTracking == 0)
                    {
                        textstring = line;
                        tk = Tokenizer.GetTokens(line);
                        lineTracking++;

                    }
                    else if(lineTracking == 1)
                    {
                        Console.WriteLine("text string: "+textstring);
                        Console.WriteLine("tk.tokenstring: "+tk.tokensString);
                        Console.WriteLine("expected tokenstring line :"+line);
                        Console.WriteLine("\n");
                        if(!tk.tokensString.Equals(line))
                        {
                            Console.WriteLine("above line is not same as below line" );
                            return ;
                        }
                        else
                        {
                            lineTracking = 0;
                        }
                    }
                }
                                
                
            }
            streamReader.Close();


    
        }
    }
}
