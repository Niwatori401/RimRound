using System;
using System.Collections.Generic;
using System.IO;

using strig = System.String;

namespace TestingRange
{
    class Program
    {
        static void Main(string[] args)
        {
            string commaDelimetedKeyvaluePairsPath = @"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Source\Stats.csv";
            string[] stringParts = File.ReadAllText(commaDelimetedKeyvaluePairsPath).Split(new string[] { ",", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<string> defNames = new List<string>();
            List<string> defValues = new List<string>();

            for (int i = 0; i < stringParts.Length; ++i) 
            {
                if (i % 2 == 0)
                    defNames.Add(stringParts[i]);
                else
                    defValues.Add(stringParts[i]);
            }

            for (int j = 0; j < defNames.Count; ++j) 
            {
                string[] replacements = new string[] { defNames[j], defValues[j] };
                string TemplateFilepath = @"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Source\Test.xml";
                string DestinationFilepath = $@"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Source\{defNames[j]}_NutritionDensityPatch.xml";
                PatchMaker.MakeSpecificPatch(TemplateFilepath, DestinationFilepath, new string[] { "DEF_REPLACE", "VALUE_REPLACE" }, replacements);
            }
        }
    }
}
