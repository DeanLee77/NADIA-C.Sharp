using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Jint;
using Nadia.C.Sharp.RuleParserFolder;

namespace Nadia.C.Sharp
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //string fileName = @"/Users/deanlee/Desktop/Programming/C#/Nadia.C.Sharp/Nadia.C.Sharp/Tokenizer_Testing.txt";
            //StreamReader file = new StreamReader(fileName);
            //string line;
            //string textString = String.Empty;
            //int lineTracking = 0;
            //Tokens tk = null;
            //while((line = file.ReadLine())!= null)
            //{
            //    line = line.Trim();
            //    var re = Regex.IsMatch(line, @"^\/.*");
            //    if (!line.Equals("") && !Regex.IsMatch(line,@"^\/.*"))
            //    {
            //        if (lineTracking == 0)
            //        {
            //            textString = line;
            //            tk = Tokenizer.GetTokens(line);
            //            lineTracking++;

            //        }
            //        else if (lineTracking == 1)
            //        {
            //            Console.WriteLine($"text string: {textString}");
            //            Console.WriteLine($"tk.tokenString: {tk.tokensString}");
            //            Console.WriteLine($"expected tokenString line : {line}");
            //            Console.WriteLine($"\n");

            //            if (!tk.tokensString.Equals(line))
            //            {
            //                Console.WriteLine($"above line is not same as below line");
            //                return;
            //            }
            //            else
            //            {
            //                lineTracking = 0;
            //            }
            //        }
            //    }

            //}
            //file.Close();

            string str = "\"this is defi String \"";
            Regex regex = new Regex(@"^([""\“])(.*)([""\”]$)");
            Match match = regex.Match(str);

            if (match.Success)
            {
                Console.WriteLine($"match.Groups: {match.Groups}");
                Console.WriteLine($"match.Groups[0].Value: {match.Groups[0].Value}");
                Console.WriteLine($"match.Groups[1].Value: {match.Groups[1].Value}");
                Console.WriteLine($"match.Groups[2].Value: {match.Groups[2].Value}");
                Console.WriteLine($"match.Groups[3].Value: {match.Groups[3].Value}");
            }

            string dateString = "11/11/1977 < 12/12/1988";
            string datePattern = @"([0-2]?[0-9]|3[0-1])/(0?[0-9]|1[0-2])/([0-9][0-9])?[0-9][0-9]|([0-9][0-9])?[0-9][0-9]/(0?[0-9]|1[0-2])/([0-2]?[0-9]|3[0-1])";
            MatchCollection mc = Regex.Matches(dateString, datePattern);


            foreach(Match mat in mc)
            {
                Console.WriteLine("--------------------------------------------");
                //for (int i = 0; i < mat.Groups.Count; i++)
                //{
                //    Console.WriteLine($"mat.Groups{i} : {mat.Groups[i]}");
                //}
                Console.WriteLine($"mat.Groups[0]: {mat.Groups[0]}");
                Console.WriteLine("--------------------------------------------");
            }


            //string line = "3+4";


            //var engine = new Jint.Engine();
            //Console.WriteLine(engine.Execute(line).GetCompletionValue());

            //DateTime dateTime = DateTime.ParseExact("11/09/2017", "dd/MM/yyyy");
            List<int> testList = new List<int>();
            testList.Add(3);
            testList.Add(5);
            testList.Add(2);
            testList.Add(9);
            testList.Add(11);
            testList.Add(13);
            testList.Add(23);
            testList.Add(6);

            List<int> var = Enumerable.Range(0, testList.Count).Where(i => testList[i] < 10).ToList();

            foreach( int num in var)
            {
                Console.WriteLine(num);
            }

            int int1 = 3;
            int int2 = 6;

            Console.WriteLine(int1 & int2);


        }
    }
}
