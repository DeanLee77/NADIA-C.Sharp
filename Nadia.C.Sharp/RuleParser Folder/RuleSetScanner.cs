using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nadia.C.Sharp.InferenceEngineFolder;
using Nadia.C.Sharp.NodeFolder;

namespace Nadia.C.Sharp.RuleParserFolder
{
    public class RuleSetScanner<T>
    {

        private IScanFeeder<T> scanFeeder = null;
        private ILineReader lineReader = null;



        public RuleSetScanner(ILineReader reader, IScanFeeder<T> feeder)
        {
            scanFeeder = feeder;
            lineReader = reader;
        }

        public void SetScanFeeder(IScanFeeder<T> scanFeeder, ILineReader lineReader)
        {
            this.scanFeeder = scanFeeder;
            this.lineReader = lineReader;
        }

        public void ScanRuleSet()
        {

            string parent = null;
            string line = null;
            string lineTrimed;
            Stack<string> parentStack = new Stack<string>();
            int previousWhitespace = 0;
            int lineNumber = 0;


            while ((line = lineReader.GetNextLine()) != null)
            {

                line = Regex.Replace(line,"\\s*(if)*\\s*$", ""); // it trims off whitespace including 'if' at the end of each line due to it could effect on calculating indentation
                lineTrimed = line.Trim();
                int currentWhitespace = 0;
                lineNumber++;

                // check the line

                // is it empty?
                if (line.Length == 0)
                {
                    parentStack.Clear();
                }
                else if (line.Trim().Substring(0, 2).Equals("//"))
                {
                    //this els if statement is to handle commenting in new line only
                    // handling commenting in rule text file needs enhancement later
                }

                // does it begin with a white space?
                else if ( Char.IsWhiteSpace(line.ToCharArray()[0]))
                {
                    currentWhitespace = line.Length - lineTrimed.Length; // calculating indentation level

                    if (lineTrimed.Length == 0) // is it a blank line? 
                    {
                        // blank line - no parent
                        parent = null;
                    }
                    else
                    {
                        int indentationDifference = previousWhitespace - currentWhitespace;
                        if (indentationDifference == -4) // this condition is for handling inputs from ACE text editor
                        {
                            indentationDifference = -1;
                        }

                        if (indentationDifference == 0 || indentationDifference > 0) //current line is at same level as previous line || current line is in upper level than previous line
                        {
                            parentStack = HandlingStackPop(parentStack, indentationDifference);

                        }
                        else if (indentationDifference < -1) // current line is not a direct child of previous line hence the format is invalid
                        {
                            //need to handle error
                            scanFeeder.HandleWarning(lineTrimed);
                            break;
                        }

                        parent = parentStack.Peek();

                        string tempLineTrimed = Regex.Replace(lineTrimed.Trim(), "^(OR\\s?|AND\\s?)?(MANDATORY|OPTIONALLY|POSSIBLY)?(\\s?NOT|\\s?KNOWN)*(NEEDS|WANTS)?", "");
                        string tempFirstKeywordsGroup = Regex.Replace(lineTrimed.Trim(), tempLineTrimed, "");
                        parentStack.Push(tempLineTrimed.Trim()); // due to lineTrimed string contains keywords such as "AND", "OR", "AND KNOWN" or "OR KNOWN" so that it needs removing those keywords for the 'parentStack'

                        // is an indented child                     
                        scanFeeder.HandleChild(parent, tempLineTrimed, tempFirstKeywordsGroup, lineNumber);
                    }

                }
                // does not begin with a white space
                else
                {
                    // is a parent
                    parentStack.Clear();
                    parent = lineTrimed;
                    scanFeeder.HandleParent(parent, lineNumber);
                    parentStack.Push(parent);
                }
                previousWhitespace = currentWhitespace;
            }
        }

        public void establishNodeSet()
        {
            NodeSet<T> ns = scanFeeder.GetNodeSet();
            ns.SetDependencyMatrix(scanFeeder.CreateDependencyMatrix());
            List<Node<T>> sortedList = TopoSort<T>.BfsTopoSort(ns.GetNodeMap(), ns.GetNodeIdMap(), ns.GetDependencyMatrix().GetDependencyMatrixArray());
            if (sortedList.Count != 0)
            {
                ns.SetNodeSortedList(sortedList);
            }
            else
            {
                scanFeeder.HandleWarning("RuleSet needs rewriting due to it is cyclic.");
            }
        }

        public void EstablishNodeSet(Dictionary<string, Record> recordMapOfNodes)
        {
            NodeSet<T> ns = scanFeeder.GetNodeSet();
            ns.SetDependencyMatrix(scanFeeder.CreateDependencyMatrix());
            List<Node<T>> sortedList = TopoSort<T>.DfsTopoSort(ns.GetNodeMap(), ns.GetNodeIdMap(), ns.GetDependencyMatrix().GetDependencyMatrixArray(), recordMapOfNodes);
            if (sortedList.Count != 0)
            {
                ns.SetNodeSortedList(sortedList);
            }
            else
            {
                scanFeeder.HandleWarning("RuleSet needs rewriting due to it is cyclic.");
            }
        }

        public Stack<String> HandlingStackPop(Stack<string> parentStack, int indentationDifference)
        {
            for (int i = 0; i < indentationDifference + 1; i++)
            {
                parentStack.Pop();
            }
            return parentStack;
        }


    }
}
