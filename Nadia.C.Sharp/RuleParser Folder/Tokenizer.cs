using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace Nadia.C.Sharp.RuleParserFolder
{
    public class Tokenizer
    {
        private static Regex spaceRegex = new Regex(@"^\s+");
        private static Regex iterateRegex = new Regex(@"^(ITERATE:([\s]*)LIST OF)(.)");
        private static Regex upperRegex = new Regex(@"^([:'’,\.\p{Lu}_\s]+(?!\p{Ll}))");
        private static Regex lowerRegex = new Regex(@"^([\p{Ll}-'’,\.\s]+(?!\d))");
        private static Regex mixedRegex = new Regex(@"^(\p{Lu}[\p{Ll}-'’,\.\s]+)+");
        private static Regex operatorRegex = new Regex(@"^([<>=]+)");
        private static Regex calculationRegex = new Regex(@"^(\()([\s|\d+(?!/.)|\w|\W]*)(\))");
        private static Regex numberRegex = new Regex(@"^(\d+)(?!/|\.|\d)+");
        private static Regex decimalNumberRegex = new Regex(@"^([\d]+\.\d+)(?!\d)");
        private static Regex dateRegex = new Regex(@"^([0-2]?[0-9]|3[0-1])/(0?[0-9]|1[0-2])/([0-9][0-9])?[0-9][0-9]|^([0-9][0-9])?[0-9][0-9]/(0?[0-9]|1[0-2])/([0-2]?[0-9]|3[0-1])");
        private static Regex urlRegex = new Regex(@"^(ht|f)tps?\:(\p{L}|\p{N}|\p{P}|^[a-fA-F0-9]+|\s)*$");
        private static Regex guidRegex = new Regex(@"^(\{?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}\}?)");
        private static Regex hashRegex = new Regex(@"^([-]?)([0-9a-fA-F]{10,}$)(?!\-)*");
        private static Regex quotedRegex = new Regex(@"^([""\“])(.*)([""\”])(\.)*");
        /*
         * the order of Pattern in the array of 'matchPatterns' is extremely important because some patterns won't work if other patterns are invoked earlier than them
         * especially 'I' pattern. 'I' pattern must come before 'U' pattern, 'Url' pattern must come before 'L' pattern with current patterns.
         */
        private static Regex[] matchPatterns = { spaceRegex, quotedRegex, iterateRegex, mixedRegex, upperRegex, urlRegex, operatorRegex, calculationRegex,
                    hashRegex, numberRegex, decimalNumberRegex, dateRegex, guidRegex, lowerRegex };
        private static string[] tokenType = { "S", "Q", "I", "M", "U", "Url", "O", "C", "Ha", "No", "De", "Da", "Id", "L" };

        public static Tokens GetTokens(string text)
        {
            List<string> tokenStringList = new List<string>();
            List<string> tokenList = new List<string>();
            string tokenString = String.Empty;
            int textLength = text.Length;

            while (textLength != 0)
            {

                for (int i = 0; i < matchPatterns.Length; i++)
                {
                    Regex regex = matchPatterns[i];
                    Match match = regex.Match(text);

                    if (match.Success == true)
                    {
                        var group = match.Groups[0].Value;

                        // ignore space tokens
                        if (!tokenType[i].Equals("S"))
                        {
                            tokenStringList.Add(tokenType[i]);
                            tokenList.Add(group.Trim());
                            tokenString += tokenType[i];
                        }

                        text = text.Substring(group.Length).Trim();
                        textLength = text.Length;
                        break;
                    }
                    if (i >= matchPatterns.Length - 1)
                    {
                        textLength = 0;
                        tokenString = "WARNING";
                    }
                }

            }

            Tokens tokens = new Tokens(tokenList, tokenStringList, tokenString);
            return tokens;

        }
    }
}
