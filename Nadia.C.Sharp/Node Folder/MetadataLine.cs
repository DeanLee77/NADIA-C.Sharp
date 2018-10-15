using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.RuleParserFolder;

namespace Nadia.C.Sharp.NodeFolder
{
    public class MetadataLine<T>: Node<T>
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

                        this.value = FactValue<T>.Parse(factValueInDate);
                    }
                    else if (this.IsDouble(lastTokenString))
                    {
                        double tempDouble = Double.Parse(tempArray[1]);
                        this.value = FactValue<T>.Parse(tempDouble);
                    }
                    else if (this.IsInteger(lastTokenString))
                    {
                        int tempInt = Int32.Parse(tempArray[1]);
                        this.value = FactValue<T>.Parse(tempInt);
                    }
                    else if (this.IsBoolean(tempArray[1]))
                    {
                        this.value = tempArray[1].ToLower().Equals("false") ? FactValue<T>.Parse(false) : FactValue<T>.Parse(true);
                    }
                    else if (this.IsHash(lastTokenString))
                    {
                        this.value = FactValue<T>.ParseHash(tempArray[1]);
                    }
                    else if (this.IsURL(lastTokenString))
                    {
                        this.value = FactValue<T>.ParseURL(tempArray[1]);
                    }
                    else if (this.IsGUID(lastTokenString))
                    {
                        this.value = FactValue<T>.ParseUUID(tempArray[1]);
                    }
                }
                else if (tempStr.Equals("AS"))
                {
                    if (tempArray[1].Equals("LIST"))
                    {
                        this.value = FactValue<T>.ParseList((T)Convert.ChangeType(new List<FactValue<T>>(), typeof(T)));
                    }
                    else
                    {
                        this.value = FactValue<T>.Parse("WARNING");
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
                        List<FactValue<T>> valueList = new List<FactValue<T>>();
                        FactValue<T> tempValue;
                        if (this.IsDate(lastTokenString)) // tempStr2 is date value
                        {

                            DateTime factValueInDate;
                            DateTime.TryParseExact(tempStr2, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out factValueInDate);
                            tempValue = FactValue<T>.Parse(factValueInDate);
                            valueList.Add(tempValue);
                        }
                        else if (this.IsDouble(lastTokenString)) //tempStr2 is double value
                        {
                            double tempDouble = Double.Parse(tempStr2);
                            tempValue = FactValue<T>.Parse(tempDouble);
                            valueList.Add(tempValue);
                        }
                        else if (this.IsInteger(lastTokenString)) //tempStr2 is integer value
                        {
                            int tempInt = Int32.Parse(tempStr2);
                            tempValue = FactValue<T>.Parse(tempInt);
                            valueList.Add(tempValue);
                        }
                        else if (this.IsHash(lastTokenString)) //tempStr2 is integer value
                        {

                            tempValue = FactValue<T>.ParseHash(tempStr2);
                            valueList.Add(tempValue);

                        }
                        else if (this.IsURL(lastTokenString)) //tempStr2 is integer value
                        {

                            tempValue = FactValue<T>.ParseURL(tempStr2);
                            valueList.Add(tempValue);

                        }
                        else if (this.IsGUID(lastTokenString)) //tempStr2 is integer value
                        {

                            tempValue = FactValue<T>.ParseUUID(tempStr2);
                            valueList.Add(tempValue);

                        }
                        else if (this.IsBoolean(tempStr2)) // tempStr2 is boolean value
                        {
                            if (tempStr2.ToLower().Equals("false"))
                            {
                                tempValue = FactValue<T>.Parse(false);

                            }
                            else
                            {
                                tempValue = FactValue<T>.Parse(true);
                            }
                            valueList.Add(tempValue);

                        }
                        else // tempStr2 is String value
                        {
                            tempValue = FactValue<T>.Parse(tempStr2);
                            valueList.Add(tempValue);
                        }

                        this.value = FactValue<T>.ParseList((T)Convert.ChangeType(valueList, typeof(T)));
                        this.value.SetDefaultValue((T)Convert.ChangeType(tempValue, typeof(T)));

                    }
                    else if (FactValueType.TEXT.ToString().Equals(tempStr))
                    {
                        this.value = FactValue<T>.Parse(tempStr2);
                        this.value.SetDefaultValue((T)Convert.ChangeType(tempStr2, typeof(T)));
                    }
                    else if (FactValueType.DATE.ToString().Equals(tempStr))
                    {
                        
                        DateTime factValueInDate;
                        DateTime.TryParseExact(tempStr2, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out factValueInDate);

                        this.value = FactValue<T>.Parse(factValueInDate);
                        this.value.SetDefaultValue((T)Convert.ChangeType(factValueInDate, typeof(T)));
                    }
                    else if (FactValueType.NUMBER.ToString().Equals(tempStr))
                    {
                        int factValueInInteger = Int32.Parse(tempStr2);
                        this.value = FactValue<T>.Parse(factValueInInteger);
                        this.value.SetDefaultValue((T)Convert.ChangeType(factValueInInteger, typeof(T)));
                    }
                    else if (FactValueType.DECIMAL.ToString().Equals(tempStr))
                    {
                        double factValueInDouble = Double.Parse(tempStr2);
                        this.value = FactValue<T>.Parse(factValueInDouble);
                        this.value.SetDefaultValue((T)Convert.ChangeType(factValueInDouble, typeof(T)));

                    }
                    else if (FactValueType.BOOLEAN.ToString().Equals(tempStr))
                    {
                        if (tempStr2.ToLower().Equals("true"))
                        {
                            this.value = FactValue<T>.Parse(true);
                            this.value.SetDefaultValue((T)Convert.ChangeType(true, typeof(T)));
                        }
                        else
                        {
                            this.value = FactValue<T>.Parse(false);
                            this.value.SetDefaultValue((T)Convert.ChangeType(false, typeof(T)));
                        }
                    }
                    else if (FactValueType.URL.ToString().Equals(tempStr))
                    {
                        this.value = FactValue<T>.ParseURL(tempStr2);
                        this.value.SetDefaultValue((T)Convert.ChangeType(tempStr2, typeof(T)));
                    }
                    else if (FactValueType.HASH.ToString().Equals(tempStr))
                    {
                        this.value = FactValue<T>.ParseHash(tempStr2);
                        this.value.SetDefaultValue((T)Convert.ChangeType(tempStr2, typeof(T)));
                    }
                    else if (FactValueType.UUID.ToString().Equals(tempStr))
                    {
                        this.value = FactValue<T>.ParseUUID(tempStr2);
                        this.value.SetDefaultValue((T)Convert.ChangeType(tempStr2, typeof(T)));
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
                        this.value = FactValue<T>.ParseList((T)Convert.ChangeType(new List<FactValue<T>>(), typeof(T)));
                    }
                    else if (FactValueType.TEXT.ToString().Equals(tempStr) || FactValueType.URL.ToString().Equals(tempStr) || FactValueType.HASH.ToString().Equals(tempStr) || FactValueType.UUID.ToString().Equals(tempStr))
                    {
                        this.value = FactValue<T>.Parse(" ");
                    }
                    else if (FactValueType.DATE.ToString().Equals(tempStr))
                    {
                        this.value = FactValue<T>.Parse(DateTime.MinValue);
                    }
                    else if (FactValueType.NUMBER.ToString().Equals(tempStr))
                    {
                        this.value = FactValue<T>.Parse(-1111);
                    }
                    else if (FactValueType.DECIMAL.ToString().Equals(tempStr))
                    {
                        this.value = FactValue<T>.Parse(-0.1111);
                    }
                    else if (FactValueType.BOOLEAN.ToString().Equals(tempStr))
                    {
                        this.value = FactValue<T>.Parse(Boolean.TrueString);
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


        public override FactValue<T> SelfEvaluate(Dictionary<string, FactValue<T>> workingMemory, Jint.Engine nashorn)
        {
            FactValue<T> fv = null;
            return fv;
        }

    }
}
