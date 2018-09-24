using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.RuleParserFolder;

namespace Nadia.C.Sharp.NodeFolder
{
    public class ExprConclusionLine<T> : Node<T>
    {
        private FactValue<T> equation;
        private string dateFormatter = @"dd/MM/yyyy";


        public ExprConclusionLine(String parentText, Tokens tokens): base(parentText, tokens)
        {

        }

        public override void Initialisation(String parentText, Tokens tokens)
        {
            this.nodeName = parentText;
            String[] tempArray = Regex.Split(parentText, "IS CALC");
            variableName = tempArray[0].Trim();
            int indexOfCInTokensStringList = tokens.tokensStringList.IndexOf("C");
            this.SetValue(tokens.tokensStringList[indexOfCInTokensStringList].Trim(), tokens.tokensList[indexOfCInTokensStringList].Trim());
            this.equation = this.value;
        }


        public FactValue<T> GetEquation()
        {
            return this.equation;
        }
        public void SetEquation(FactValue<T> newEquation)
        {
            this.equation = newEquation;
        }

       

        public override LineType GetLineType()
        {
            return LineType.EXPR_CONCLUSION;
        }


        public override FactValue<T> SelfEvaluate(Dictionary<string, FactValue<T>> workingMemory, Jint.Engine jint)
        {
            /*
             * calculation can only handle int, double(long) and difference in years between two dates at the moment.
             * if difference in days or months is required then new 'keyword' must be introduced such as 'Diff Years', 'Diff Days', or 'Diff Months'
             */
            string equationInString = this.equation.GetValue().ToString();
            string pattern = @"[-+/*()?:;,.""](\s*)";
            string datePattern = @"([0-2]?[0-9]|3[0-1])/(0?[0-9]|1[0-2])/([0-9][0-9])?[0-9][0-9]|([0-9][0-9])?[0-9][0-9]/(0?[0-9]|1[0-2])/([0-2]?[0-9]|3[0-1])";


            /*
             * logic for this is as follows;
             *  1. replace all variables with actual values from 'workingMemory'
             *  2. find out if equation is about date (difference in years) calculation or not
             *  3. if it is about date then call 'java.time.LocalDate'and 'java.time.temporal.ChronoUnit' package then do the calculation
             *  3-1. if it is about int or double(long) then use plain Javascript
             *  
             */

            string script = equationInString;
            string tempScript = script;

            if (Regex.IsMatch(equationInString, pattern))
            {
                string[] tempArray = Regex.Split(equationInString, pattern);
                int tempArrayLength = tempArray.Length;
                string tempItem;
                for (int i = 0; i < tempArrayLength; i++)
                {
                    tempItem = tempArray[i];
                    if (!String.IsNullOrEmpty(tempItem.Trim()) && workingMemory[tempItem.Trim()] != null)
                    {
                        FactValue<T> tempFv = workingMemory[tempItem.Trim()];
                        if (tempFv.GetValue().GetType().FullName.Equals(DateTime.Now.GetType().FullName))
                        {
                            /*
                             * below line is temporary solution.
                             * Within next iteration it needs to be that this node should take dateFormatter for its constructor to determine which date format it needs
                             */
                            string tempStr = DateTime.ParseExact(tempFv.GetValue().ToString(), dateFormatter, CultureInfo.InvariantCulture).ToString();
                            tempScript = tempScript.Replace(tempScript.Trim(), tempStr);
                        }
                        else
                        {
                            tempScript = tempScript.Replace(tempItem.Trim(), workingMemory[tempItem.Trim()].GetValue().ToString().Trim());
                        }
                    
                    }
                }
            }
        
            Match dateMatcher = Regex.Match(tempScript, datePattern);
            List<string> dateStringList = new List<string>();
            while(dateMatcher.Success)
            {
                dateStringList.Add(dateMatcher.Value);
            }
            // if dateStringList.size() == 0 then there is no string in date format
            script = tempScript;  
            //if(dateStringList.size() != 0) // case of date calculation
            //{
            //    string[] date1Array = dateStringList.get(0).trim().split("/");
            //    string[] date2Array = dateStringList.get(1).trim().split("/");
            //    script = "var localDate = java.time.LocalDate; var chronoUnit = java.time.temporal.ChronoUnit; var diffYears = chronoUnit.YEARS.between(localDate.of("+date2Array[2].trim()+","+date2Array[1].trim()+","+date2Array[0].trim()+"), localDate.of("+date1Array[2].trim()+","+date1Array[1].trim()+","+date1Array[0].trim()+")); diffYears;";
            //}
    //      else // case of int or double calculation
    //      {
    //           don't need to do anything due to script itself can be evaluated as it is
    //      }
        

            FactValue<T> returnValue = null;

    //        try {
    //                string nashornResult = nashorn.eval(script).toString();
    //                switch(Tokenizer.getTokens(nashornResult).tokensString)
    //                {
    //                    case "No":
    //                        returnValue = FactValue.parse(Integer.parseInt(nashornResult));
    //                        break;
    //                    case "De":
    //                        returnValue = FactValue.parse(Double.parseDouble(nashornResult));
    //                        break;
    ////                  there is no function for outcome to be a date at the moment  E.g. The determination IS CALC (enrollment date + 5 days)
    ////                  case "Da":
    ////                      DateTimeFormatter formatter = DateTimeFormatter.ofPattern("dd/MM/yyyy");
    ////                      LocalDate factValueInDate = LocalDate.parse(nashornResult, formatter);
    ////                      returnValue = FactValue.parse(factValueInDate);
    ////                      break;
            //            default:
            //            if(this.isBoolean(nashornResult))
            //            {
            //                returnValue = FactValue.parse(nashornResult);
            //            }
            //            else 
            //            {
            //                returnValue = FactValue.parse(nashornResult);
            //            }
            //                break;

            //        }
            //} catch (ScriptException e) {
            //    e.printStackTrace();
            //}
            return returnValue;
        }

    }
}
