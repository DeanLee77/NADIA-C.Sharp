using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using Nadia.C.Sharp.RuleParserFolder;

namespace Nadia.C.Sharp.FactValueFolder
{
    public abstract class FactValue
    {
        public abstract FactValueType GetFactValueType();


        public static FactBooleanValue Parse(bool boolValue)
        {
            return new FactBooleanValue(boolValue);
        }

        public static FactDateValue Parse(DateTime dateValue)
        {
            return new FactDateValue(dateValue);
        }

        public static FactDefiStringValue ParseDefiString(string stringValue)
        {
            return new FactDefiStringValue(stringValue);
        }

        public static FactDoubleValue Parse(double doubleValue)
        {
            return new FactDoubleValue(doubleValue);
        }

        public static FactHashValue ParseHash(string hashValue)
        {
            return new FactHashValue(hashValue);
        }

        public static FactIntegerValue Parse(int intValue)
        {
            return new FactIntegerValue(intValue);
        }

        public static FactListValue Parse(List<FactValue> listValue)
        {
            return new FactListValue(listValue);
        }

        public static FactStringValue Parse(string stringValue)
        {
            return new FactStringValue(stringValue);
        }

        public static FactURLValue ParseURL(string urlValue)
        {
            return new FactURLValue(urlValue);
        }

        public static FactUUIDValue ParseUUID(string uuidValue)
        {
            return new FactUUIDValue(uuidValue);
        }

        public static FactValueType? FindFactValueType(string value)
        {
            FactValueType? factValueType = null;
            Tokens tk = Tokenizer.GetTokens(value);
            switch (tk.tokensString)
            {
                case "No":
                    factValueType = FactValueType.INTEGER;
                    break;
                case "Do":
                    factValueType = FactValueType.DOUBLE;
                    break;
                case "Da":
                    factValueType = FactValueType.DATE;
                    break;
                case "Url":
                    factValueType = FactValueType.URL;
                    break;
                case "Id":
                    factValueType = FactValueType.UUID;
                    break;
                case "Ha":
                    factValueType = FactValueType.HASH;
                    break;
                case "Q":
                    factValueType = FactValueType.DEFI_STRING;
                    break;
                case "L":
                case "M":
                case "U":
                case "C":
                    if (IsBoolean(value))
                    {
                        factValueType = FactValueType.BOOLEAN;
                    }
                    else
                    {
                        Regex regex = new Regex(@"^([""\“])(.*)([""\”]$)");
                        Match match = regex.Match(value);

                        if (match.Success)
                        {
                            factValueType = FactValueType.DEFI_STRING;
                        }
                        else
                        {
                            factValueType = FactValueType.STRING;
                        }
                    }
                    break;
            }
            return factValueType;
        }

        public static bool IsBoolean(string str)
        {
            return Regex.IsMatch(str, @"[FfAaLlSsEe]+") || Regex.IsMatch(str, @"[TtRrUuEe]+");
        }



        public static FactValue GenerateFactValue(FactValueType factValueType, string str)
        {
            FactValue factValue = null;

            switch(factValueType)
            {
                case FactValueType.BOOLEAN:
                    factValue = Parse(Boolean.Parse(str));
                    break;
                case FactValueType.DATE:
                    DateTime dateValue;
                    DateTime.TryParseExact(str, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dateValue);

                    factValue = Parse(dateValue);
                    break;
                case FactValueType.DEFI_STRING:
                    factValue = ParseDefiString(str);
                    break;
                case FactValueType.DOUBLE:
                    factValue = Parse(Double.Parse(str));
                    break;
                case FactValueType.HASH:
                    factValue = ParseHash(str);
                    break;
                case FactValueType.UUID:
                    factValue = ParseUUID(str);
                    break;
                case FactValueType.URL:
                    factValue = ParseURL(str);
                    break;
                case FactValueType.INTEGER:
                    factValue = Parse(Int32.Parse(str));
                    break;
                case FactValueType.STRING:
                    factValue = Parse(str);
                    break;
            }
            return factValue;
        }


        public static string GetValueInString(FactValueType factValueType, FactValue factValue)
        {
            string value = null;
            switch(factValueType)
            {
                case FactValueType.BOOLEAN:
                    value = ((FactBooleanValue)factValue).GetValue().ToString();
                    break;
                case FactValueType.DATE:
                    value = ((FactDateValue)factValue).GetValue().ToString();
                    break;
                case FactValueType.DEFI_STRING:
                    value = ((FactDefiStringValue)factValue).GetValue();
                    break;
                case FactValueType.DOUBLE:
                    value = ((FactDoubleValue)factValue).GetValue().ToString();
                    break;
                case FactValueType.HASH:
                    value = ((FactHashValue)factValue).GetValue();
                    break;
                case FactValueType.URL:
                    value = ((FactURLValue)factValue).GetValue();
                    break;
                case FactValueType.UUID:
                    value = ((FactUUIDValue)factValue).GetValue();
                    break;
                case FactValueType.INTEGER:
                    value = ((FactIntegerValue)factValue).GetValue().ToString();
                    break;
                case FactValueType.STRING:
                    value = ((FactStringValue)factValue).GetValue();
                    break;

            }
            return value;
        }

    }
}
