using System;
namespace Nadia.C.Sharp.NodeFolder
{
    public class DependencyType
    {

        private static int mandatory = 64; // 1000000
        private static int optional = 32;  // 0100000
        private static int possible = 16;  // 0010000
        private static int and = 8;        // 0001000
        private static int or = 4;         // 0000100
        private static int not = 2;        // 0000010
        private static int known = 1;      // 0000001
        
        public static int GetMandatory()
        {
            return mandatory;
        }
        
        public static int GetOptional()
        {
            return optional;
        }
        
        public static int GetPossible()
        {
            return possible;
        }
        
        public static int GetAnd()
        {
            return and;
        }
        
        public static int GetOr()
        {
            return or;
        }
        
        public static int GetNot()
        {
            return not;
        }
        
        public static int GetKnown()
        {
            return known;
        }
    
    }
}
