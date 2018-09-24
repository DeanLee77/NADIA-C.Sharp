using System;
using System.Collections.Generic;

namespace Nadia.C.Sharp.RuleParserFolder
{
    public class Tokens
    {
        public List<string> tokensList;
        public List<string> tokensStringList;
        public string tokensString;

        public Tokens(List<string> tl, List<string> tsl, string ts)
        {
            tokensList = tl;
            tokensStringList = tsl;
            tokensString = ts;
        }

        public Tokens(){
            
        }
    }
}
