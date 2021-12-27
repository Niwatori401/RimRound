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
            string[] patchTemplates = new string[] 
            {
                "RimRound_AdjustAlignWithHeadTag.xml",
                "RimRound_AddComps.xml",
                "RimRound_AlignBodyPart.xml",
                "RimRound_RemoveBodySpecificOffsets.xml"
            };


            string templateFilePath = @"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\PatchMaker\RimRound_RemoveBodySpecificOffsets.xml~";
            string destinationFilePath = @"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\PatchMaker\RimRound_RemoveBodySpecificOffsets.xml";
            string csvFilePath = @"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Source\Stats4.csv";
            int flairArgs = 2;

            PatchMaker.MakePatchWithCSV(templateFilePath, destinationFilePath, csvFilePath, flairArgs);
        }
    }
}
