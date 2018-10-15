using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.RuleParserFolder;
using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Nadia.C.Sharp
{
    [TestFixture()]
    public class NUnitTestClass
    {
        [Test()]
        public void TestCase<T>()
        {

        
            FactValue<T> fv = FactValue<T>.Parse(true);
            Console.WriteLine("fv.getType(): " + fv.GetType());
            // TODO Auto-generated method stub
            string fileName = "Tokenizer_Testing.txt";

            StreamReader streamReader = new StreamReader(fileName);


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


    
        }
    }
}
