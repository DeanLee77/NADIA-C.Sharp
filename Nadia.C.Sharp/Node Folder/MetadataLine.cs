using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.RuleParserFolder;

namespace Nadia.C.Sharp.NodeFolder
{
    public class MetadataLine: Node
    {
        //  Pattern metaPatternMatcher = Pattern.compile("^ULU?[OU]?[NoDaMLDe]?");
        /*
         * Meta type pattern list
         * 1. ULU[NoDaMLDe]
         * 2. U[NoDaMLDe]
         * 3. U
         */
        private MetaType metaType;
        private String name;

        public MetadataLine(string parentText, Tokens tokens): base(parentText, tokens)
        {

        }


        public override void Initialisation(string parentText, Tokens tokens)
        {
            this.name = parentText;
            this.SetMetaType(parentText);
            Regex regex;
            Match match;

            switch (this.metaType)
            {
                case MetaType.FIXED:

                    regex = new Regex("^(FIXED)(.*)(\\s[AS|IS]\\s*.*)");
                    match = regex.Match(parentText);
                   
                    if(match.Success)
                    {
                        this.variableName = match.Groups[2].ToString().Trim();
                        SetValue(match.Groups[3].ToString().Trim(), tokens);
                    }
                    break;
                case MetaType.INPUT:
                    regex = new Regex("^(INPUT)(.*)(AS)(.*)[(IS)(.*)]?");
                    match = regex.Match(parentText);
                    if (match.Success)
                    {
                        this.variableName = match.Groups[2].ToString().Trim();
                        SetValue(match.Groups[4].ToString().Trim(), tokens);
                    }
                    break;

                default:
                    break;
            }
        }


        public void SetValue(string valueInString, Tokens tokens)
        {
            int tokenStringListSize = tokens.tokensStringList.Count;
            string lastTokenString = tokens.tokensStringList[tokenStringListSize - 1];
            string[] tempArray = Regex.Split(valueInString, " ");
            string tempStr = tempArray[0];

            if (metaType.Equals(MetaType.FIXED))
            {
                if (tempStr.Equals("IS"))
                {
                    if (this.IsDate(lastTokenString))
                    {
                        DateTime factValueInDate;
                        DateTime.TryParseExact(tempArray[1], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out factValueInDate);

                        this.value = FactValue.Parse(factValueInDate);
                    }
                    else if (this.IsDouble(lastTokenString))
                    {
                        double tempDouble = Double.Parse(tempArray[1]);
                        this.value = FactValue.Parse(tempDouble);
                    }
                    else if (this.IsInteger(lastTokenString))
                    {
                        int tempInt = Int32.Parse(tempArray[1]);
                        this.value = FactValue.Parse(tempInt);
                    }
                    else if (this.IsBoolean(tempArray[1]))
                    {
                        this.value = tempArray[1].ToLower().Equals("false") ? FactValue.Parse(false) : FactValue.Parse(true);
                    }
                    else if (this.IsHash(lastTokenString))
                    {
                        this.value = FactValue.ParseHash(tempArray[1]);
                    }
                    else if (this.IsURL(lastTokenString))
                    {
                        this.value = FactValue.ParseURL(tempArray[1]);
                    }
                    else if (this.IsGUID(lastTokenString))
                    {
                        this.value = FactValue.ParseUUID(tempArray[1]);
                    }
                }
                else if (tempStr.Equals("AS"))
                {
                    if (tempArray[1].Equals("LIST"))
                    {
                        this.value = FactValue.Parse(new List<FactValue>());
                    }
                    else
                    {
                        this.value = FactValue.Parse("WARNING");
                    }
                }

            }
            else if (metaType.Equals(MetaType.INPUT))
            {
                if (tempArray.Length > 1)
                {

                    /*
                     * within this case 'DefaultValue' will be set due to the statement format is as follows;
                     * 'A AS 'TEXT' IS B'
                     * and 'A' is variable, 'TEXT' is a type of variable, and 'B' is a default value.
                     * if the type is 'LIST' then variable is a list then the factValue has a default value.
                     */
                    String tempStr2 = tempArray[2];

                    if (FactValueType.LIST.ToString().Equals(tempStr))
                    {
                        List<FactValue> valueList = new List<FactValue>();
                        FactValue tempValue;
                        if (this.IsDate(lastTokenString)) // tempStr2 is date value
                        {

                            DateTime factValueInDate;
                            DateTime.TryParseExact(tempStr2, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out factValueInDate);
                            tempValue = FactValue.Parse(factValueInDate);
                            valueList.Add(tempValue);
                        }
                        else if (this.IsDouble(lastTokenString)) //tempStr2 is double value
                        {
                            double tempDouble = Double.Parse(tempStr2);
                            tempValue = FactValue.Parse(tempDouble);
                            valueList.Add(tempValue);
                        }
                        else if (this.IsInteger(lastTokenString)) //tempStr2 is integer value
                        {
                            int tempInt = Int32.Parse(tempStr2);
                            tempValue = FactValue.Parse(tempInt);
                            valueList.Add(tempValue);
                        }
                        else if (this.IsHash(lastTokenString)) //tempStr2 is integer value
                        {

                            tempValue = FactValue.ParseHash(tempStr2);
                            valueList.Add(tempValue);

                        }
                        else if (this.IsURL(lastTokenString)) //tempStr2 is integer value
                        {

                            tempValue = FactValue.ParseURL(tempStr2);
                            valueList.Add(tempValue);

                        }
                        else if (this.IsGUID(lastTokenString)) //tempStr2 is integer value
                        {

                            tempValue = FactValue.ParseUUID(tempStr2);
                            valueList.Add(tempValue);

                        }
                        else if (this.IsBoolean(tempStr2)) // tempStr2 is boolean value
                        {
                            if (tempStr2.ToLower().Equals("false"))
                            {
                                tempValue = FactValue.Parse(false);

                            }
                            else
                            {
                                tempValue = FactValue.Parse(true);
                            }
                            valueList.Add(tempValue);

                        }
                        else // tempStr2 is String value
                        {
                            tempValue = FactValue.Parse(tempStr2);
                            valueList.Add(tempValue);
                        }

                        this.value = FactValue.Parse(valueList);
                        ((FactListValue)this.value).SetDefaultValue(tempValue);

                    }
                    else if (FactValueType.TEXT.ToString().Equals(tempStr))
                    {
                        this.value = FactValue.Parse(tempStr2);
                    }
                    else if (FactValueType.DATE.ToString().Equals(tempStr))
                    {
                        
                        DateTime factValueInDate;
                        DateTime.TryParseExact(tempStr2, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out factValueInDate);

                        this.value = FactValue.Parse(factValueInDate);
                    }
                    else if (FactValueType.NUMBER.ToString().Equals(tempStr))
                    {
                        int factValueInInteger = Int32.Parse(tempStr2);
                        this.value = FactValue.Parse(factValueInInteger);
                    }
                    else if (FactValueType.DECIMAL.ToString().Equals(tempStr))
                    {
                        double factValueInDouble = Double.Parse(tempStr2);
                        this.value = FactValue.Parse(factValueInDouble);

                    }
                    else if (FactValueType.BOOLEAN.ToString().Equals(tempStr))
                    {
                        if (tempStr2.ToLower().Equals("true"))
                        {
                            this.value = FactValue.Parse(true);
                        }
                        else
                        {
                            this.value = FactValue.Parse(false);
                        }
                    }
                    else if (FactValueType.URL.ToString().Equals(tempStr))
                    {
                        this.value = FactValue.ParseURL(tempStr2);
                    }
                    else if (FactValueType.HASH.ToString().Equals(tempStr))
                    {
                        this.value = FactValue.ParseHash(tempStr2);
                    }
                    else if (FactValueType.UUID.ToString().Equals(tempStr))
                    {
                        this.value = FactValue.ParseUUID(tempStr2);
                    }
                }
                else
                {
                    /*
                     * case of the statement does not have value, only contains a type of the variable
                     * so that the value will not have any default values
                     */
                    if (FactValueType.LIST.ToString().Equals(tempStr))
                    {
                        this.value = FactValue.Parse(new List<FactValue>());
                    }
                    else if (FactValueType.TEXT.ToString().Equals(tempStr) || FactValueType.URL.ToString().Equals(tempStr) || FactValueType.HASH.ToString().Equals(tempStr) || FactValueType.UUID.ToString().Equals(tempStr))
                    {
                        this.value = FactValue.Parse(" ");
                    }
                    else if (FactValueType.DATE.ToString().Equals(tempStr))
                    {
                        this.value = FactValue.Parse(DateTime.MinValue);
                    }
                    else if (FactValueType.NUMBER.ToString().Equals(tempStr))
                    {
                        this.value = FactValue.Parse(-1111);
                    }
                    else if (FactValueType.DECIMAL.ToString().Equals(tempStr))
                    {
                        this.value = FactValue.Parse(-0.1111);
                    }
                    else if (FactValueType.BOOLEAN.ToString().Equals(tempStr))
                    {
                        this.value = FactValue.Parse(Boolean.TrueString);
                    }

                }
            }

        }

        public void SetMetaType(string parentText)
        {
            Enum.GetValues(typeof(MetaType)).Cast<MetaType>().ToList().ForEach((metaType) =>
            {
                if(parentText.Contains(metaType.ToString()))
                {
                    this.metaType = metaType;
                }
            });
        }
        public MetaType GetMetaType()
        {
            return this.metaType;
        }

        public String GetName()
        {
            return this.name;
        }


        public override LineType GetLineType()
        {
            // TODO Auto-generated method stub
            return LineType.META;
        }


        public override FactValue SelfEvaluate(Dictionary<string, FactValue> workingMemory, Jint.Engine nashorn)
        {
            FactValue fv = null;
            return fv;
        }

    }
}
