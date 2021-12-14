using System;

namespace TestingRange
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetEquivalentBodyTypeDef("F_070_Enormous_Ratkin"));

        }

        public static string GetEquivalentBodyTypeDef(string raceSpecificDef)
        {
            int endPos = raceSpecificDef.LastIndexOf('_');


            string cleanedDefName = raceSpecificDef.Substring(0, endPos);

            return cleanedDefName;
        }
    }
}
