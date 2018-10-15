using System;
using System.Collections.Generic;
using System.Linq;
using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.RuleParserFolder;

namespace Nadia.C.Sharp.NodeFolder
{
    public class ValueConclusionLine<T> : Node<T>
    {
        private bool isPlainStatementFormat;

        /*
         * ValueConclusionLine format is as follows;
         *  1. 'A-statement IS B-statement';
         *  2. 'A-item name IS IN LIST: B-list name'; or
         *  3. 'A-statement'(plain statement line) including statement of 'A' type from a child node of ExprConclusionLine type which are 'NEEDS' and 'WANTS'.
         * When the inference engine reaches at a ValueConclusionLine and needs to ask a question to a user, 
         * this rule must be either 'A (plain statement)' or 'A IS B' format due to the reason that other than the two format cannot be a parent rule.
         * Hence, the question can be from either variableName or ruleName, and a result of the question will be inserted into the workingMemory.
         * However, when the engine reaches at the line during forward-chaining then the key for the workingMemory will be a ruleName,
         * and value for the workingMemory will be set as a result of propagation.
         * 
         * If the rule statement is in a format of 'A-statement' then a default value of variable 'value' will be set as 'false'
         * 
         */
        public ValueConclusionLine(string nodeText, Tokens tokens) : base(nodeText, tokens)
        {

        }


        public override void Initialisation(string nodeText, Tokens tokens)
        {
            int tokensStringListSize = tokens.tokensStringList.Count; //tokens.tokensStringList.size is same as tokens.tokensList.size

            this.isPlainStatementFormat = !tokens.tokensList.Any((s) => s.Contains("IS")); //this will exclude 'IS' and 'IS IN  LIST:' within the given 'tokens'


            string lastToken = null;
            if (!isPlainStatementFormat) //the line must be a parent line in this case other than a case of the rule contains 'IS IN LIST:'
            {

                this.variableName = nodeText.Substring(0, nodeText.IndexOf("IS")).Trim();
                lastToken = tokens.tokensList[tokensStringListSize - 1];
            }
            else //this is a case of that the line is in a 'A-statement' format
            {

                this.variableName = nodeText;
                lastToken = "false";

            }

            this.nodeName = nodeText;
            string lastTokenString = tokens.tokensStringList[tokensStringListSize - 1];
            this.SetValue(lastTokenString, lastToken);
        }

        public bool GetIsPlainStatementFormat()
        {
            return this.isPlainStatementFormat;
        }


        public override LineType GetLineType()
        {
            return LineType.VALUE_CONCLUSION;
        }


        public override FactValue<T> SelfEvaluate(Dictionary<string, FactValue<T>> workingMemory, Jint.Engine nashorn)
        {
            FactValue<T> fv = null;
            /*
             * Negation and Known type are a part of dependency 
             * hence, only checking its variableName value against the workingMemory is necessary.
             * type is as follows;
             *  1. the rule is a plain statement
             *  2. the rule is a statement of 'A IS B'
             *  3. the rule is a statement of 'A IS IN LIST: B'
             *  4. the rule is a statement of 'needs(wants) A'. this is from a child node of ExprConclusionLine type 
             */

            if (!this.isPlainStatementFormat)
            {
                if (tokens.tokensList.Any(s => s.Equals("IS")))
                {
                    fv = this.value;
                }
                else if (tokens.tokensList.Any((s)=> s.Equals("IS IN LIST:")))
                {
                    bool lineValue = false;
                    string listName = this.GetFactValue().GetValue().ToString();
                    if (workingMemory[listName] != null)
                    {
                        FactValue<T> variableValueFromWorkingMemory = workingMemory[this.variableName];
                        lineValue = variableValueFromWorkingMemory != null ?
                        (workingMemory[listName].GetValue() as List<T>).Any((item) =>(item as FactStringValue<T>).GetValue().Equals(variableValueFromWorkingMemory.GetValue().ToString()))
                        :
                        (workingMemory[listName].GetValue() as List<T>).Any((item) => (item as FactStringValue<T>).GetValue().Equals(this.variableName));
                    }

                    fv = FactValue<T>.Parse(lineValue);
                }
            }

            return fv;
        }

    }
}
