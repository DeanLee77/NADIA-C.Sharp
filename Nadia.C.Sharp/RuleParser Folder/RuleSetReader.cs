using System;
using System.IO;
using System.Text;

namespace Nadia.C.Sharp.RuleParserFolder
{
    public class RuleSetReader : ILineReader
    {

    
        private System.IO.StreamReader streamReader;
        
        public void SetFileSource(string filePath)
        {
            streamReader = new System.IO.StreamReader(filePath);
        }
        
        public void SetStreamSource(MemoryStream memoryStream)
        {
            streamReader = new StreamReader(memoryStream);
        }
        
        public void SetStringSource(string text)
        {
            //convert a given text into Stream
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            //read the Stream with StreamReader
            streamReader = new StreamReader(mStream);
        }
    
        public string GetNextLine()
        {
            string line = null;
            try 
            {
                line = streamReader.ReadLine();
            } 
            catch (Exception e) 
            {
                // TODO Auto-generated catch block
                Console.WriteLine(e.Message);
            }
            
            if(line == null)
            {
                try 
                {
                    streamReader.Close();
                } 
                catch (IOException e) 
                {
                    // TODO Auto-generated catch block
                    Console.WriteLine(e.Message);
                }
            }
            
            return line;
        }
        


    }
}
