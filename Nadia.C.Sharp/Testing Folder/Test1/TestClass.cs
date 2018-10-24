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

                    ie.FeedAnswerToNode(nextQuestionNode, question, FactValue.GenerateFactValue(answer), ass);
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
