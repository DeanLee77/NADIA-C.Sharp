using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.RuleParserFolder;

namespace Nadia.C.Sharp.NodeFolder
{
    public class ComparisonLine : Node
    {
        private string operatorString;

        private string lhs;
        private FactValue rhs;

        public ComparisonLine(string childText, Tokens tokens) : base(childText, tokens)
        {

        }


        public override void Initialisation(string childText, Tokens tokens)
        {

            /*
             * this line pattern is as (^[ML]+)(O)([MLNoDaDeHaUrlId]*$)
             */

            this.nodeName = childText;
            /*
             * In javascript engine '=' operator means assigning a value, hence if the operator is '=' then it needs to be replaced with '=='. 
             */
            int operatorIndex = tokens.tokensStringList.IndexOf("O");
            this.operatorString = tokens.tokensList[operatorIndex].Equals("=") ? "==" : tokens.tokensList[operatorIndex];

            if (operatorString.Equals("=="))
            {
                this.variableName = childText.Split('=')[0].Trim();
            }
            else
            {
                string[] splitor = new string[] { this.operatorString };

                this.variableName = childText.Split(splitor, StringSplitOptions.None)[0].Trim();
            }
            this.lhs = variableName;

            int tokensStringListSize = tokens.tokensStringList.Count;
            String lastToken = tokens.tokensList[tokensStringListSize - 1];
            String lastTokenString = tokens.tokensStringList[tokensStringListSize - 1];
            this.SetValue(lastTokenString, lastToken);
            this.rhs = this.value;


        }



        public String GetRuleName()
        {
            return this.nodeName;
        }

        public String GetLHS()
        {
            return this.lhs;
        }

        public FactValue GetRHS()
        {
            return this.rhs;
        }

        public override LineType GetLineType()
        {
            return LineType.COMPARISON;
        }


        public override FactValue SelfEvaluate(Dictionary<string, FactValue> workingMemory, Jint.Engine jint)
        {

            /*
             * Negation type can only be used for this line type
             * 
             */

            FactValue workingMemoryLhsValue = workingMemory.ContainsKey(this.variableName) ? workingMemory[this.variableName] : null;
            string rhsValueInString = FactValue.GetValueInString(this.GetRHS().GetFactValueType(), this.GetRHS());

            FactValue workingMemoryRhsValue = this.GetRHS().GetFactValueType().Equals(FactValueType.STRING) ?
                                                  workingMemory[rhsValueInString]
                                                     :
                                                      this.GetRHS();

            String script = "";

            /*
             * There will NOT be the case of that workingMemoryRhsValue is null because the node must be in following format;
             * - A = 12231 (int or double)
             * - A = Adam sandler (String)
             * - A = 11/11/1977 (Date)
             * - A = 123123dfae1421412aer(Hash)
             * - A = 1241414-12421312-142421312(UUID)
             * - A = true(Boolean)
             * - A = www.aiBrain.com(URL)
             * - A = B(another variable) 
             */

            /*
             * if it is about date comparison then string of 'script' needs rewriting
             */
            if ((workingMemoryLhsValue != null && workingMemoryLhsValue.GetFactValueType().Equals(FactValueType.DATE)) || (workingMemoryRhsValue != null && workingMemoryRhsValue.GetFactValueType().Equals(FactValueType.DATE)))
            {
                Boolean returnValue;
                switch (this.operatorString)
                {
                    case ">":
                        returnValue = (((FactDateValue)workingMemoryLhsValue).GetValue()).CompareTo((((FactDateValue)workingMemoryRhsValue).GetValue())) > 0 ? true : false;
                        return FactValue.Parse(returnValue);

                    case ">=":
                        returnValue = (((FactDateValue)workingMemoryLhsValue).GetValue()).CompareTo((((FactDateValue)workingMemoryRhsValue).GetValue())) >= 0 ? true : false;
                        return FactValue.Parse(returnValue);

                    case "<":
                        returnValue = (((FactDateValue)workingMemoryLhsValue).GetValue()).CompareTo((((FactDateValue)workingMemoryRhsValue).GetValue())) < 0 ? true : false;
                        return FactValue.Parse(returnValue);

                    case "<=":
                        returnValue = (((FactDateValue)workingMemoryLhsValue).GetValue()).CompareTo((((FactDateValue)workingMemoryRhsValue).GetValue())) <= 0 ? true : false;
                        return FactValue.Parse(returnValue);

                }
                //          script = "new Date("+((FactDateValue)workingMemoryLhsValue).getValue().getYear()+"/"+((FactDateValue)workingMemoryLhsValue).getValue().getMonthValue()+"/"+((FactDateValue)workingMemoryLhsValue).getValue().getDayOfMonth()+")"+operator+"new Date("+((FactDateValue)workingMemoryRhsValue).getValue().getYear()+"/"+((FactDateValue)workingMemoryRhsValue).getValue().getMonthValue()+"/"+((FactDateValue)workingMemoryRhsValue).getValue().getDayOfMonth()+");" ;
            }
            else if (workingMemoryLhsValue != null && (workingMemoryLhsValue.GetFactValueType().Equals(FactValueType.DECIMAL) || workingMemoryLhsValue.GetFactValueType().Equals(FactValueType.DOUBLE)
                      || workingMemoryLhsValue.GetFactValueType().Equals(FactValueType.INTEGER) || workingMemoryLhsValue.GetFactValueType().Equals(FactValueType.NUMBER)))
            {
                script = FactValue.GetValueInString(workingMemoryLhsValue.GetFactValueType(), workingMemoryLhsValue) + operatorString + FactValue.GetValueInString(workingMemoryRhsValue.GetFactValueType(), workingMemoryRhsValue);
            }
            else
            {
                if (workingMemoryRhsValue != null && workingMemoryLhsValue != null)
                {
                    script = "'" + FactValue.GetValueInString(workingMemoryLhsValue.GetFactValueType(), workingMemoryLhsValue) + "' " + operatorString + " '" + FactValue.GetValueInString(workingMemoryRhsValue.GetFactValueType(), workingMemoryRhsValue) + "'";
                }

            }
            Boolean result;
            FactValue fv = null;
            if (workingMemoryRhsValue != null && workingMemoryLhsValue != null)
            {
                 result = Convert.ToBoolean(jint.Execute(script).GetCompletionValue().ToString());
                fv = FactValue.Parse(result);
            }


            return fv;
        }
    }
}
