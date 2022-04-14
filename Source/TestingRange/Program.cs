using System;
using System.Collections.Generic;
using System.IO;
using Verse;

using strig = System.String;

namespace TestingRange
{
    class Program
    {


        static void Main(string[] args)
        {

            PatchMode patchMode = PatchMode.None;

            while (patchMode == PatchMode.None)
            { 
                Console.WriteLine("Select patch mode: \n" +
                    "1: Wipe and regenerate\n" +
                    "2: Only regenerate\n" +
                    "3: Only wipe\n");

                string response = Console.ReadLine();

                if (response[0] == '1')
                {
                    patchMode = PatchMode.WipeAndRefresh;
                }
                else if (response[0] == '2')
                {
                    patchMode = PatchMode.Refresh;
                }
                else if (response[0] == '3')
                {
                    patchMode = PatchMode.Wipe;
                }
                else 
                {
                    Console.WriteLine("Please choose a valid patch mode.\n");
                }
            }
            List<PatchData> patchSets = new List<PatchData>()
            {
                new PatchData("RimRound_AdjustAlignWithHeadTag",                    "alignWithHeadPatches", 3),
                new PatchData("RimRound_AddComps",                                  "compsPatches", 2),
                new PatchData("RimRound_AlignBodyPart",                             "bodyPartAlignmentPatches", 3),
                new PatchData("RimRound_RemoveBodySpecificOffsets",                 "removeBodySpecificOffsetsPatches", 3),
                new PatchData("RimRound_BodyTypeSpecificAlignmentPatch",            "bodyTypeSpecificAlignmentPatch", 4),
                new PatchData("RimRound_RemoveGeneralOffset",                       "removeGeneralOffsetPatch", 3),
                new PatchData("RimRound_SetBodyAddonDrawsize",                      "setBodyAddonDrawsize", 3),
                new PatchData("RimRound_RemoveScaleWithBodyDrawsizeTag",            "removeScaleWithBodyDrawsizeTag", 3),
                new PatchData("RimRound_RemoveBodySpecificPortraitOffset",          "removeBodySpecificPortraitOffsetsPatches", 3),
                new PatchData("RimRound_RemoveGeneralHeadOffset",                   "removeGeneralHeadOffset", 2),
                new PatchData("RimRound_RemoveSpecificGraphicPathHeadOffset",       "removeSpecificGraphicPathHeadOffset", 3),
                new PatchData("RimRound_RemoveDefaultOffset",                       "removeDefaultOffset", 3),
                new PatchData("RimRound_AlignBodyPartAbstractDef",                  "bodyPartAlignmentPatchesAbstract", 3),
                new PatchData("RimRound_BodyTypeSpecificAlignmentPatchAbstractDef", "bodyTypeSpecificAlignmentPatchAbstract", 4),
            };

            // TemplateName, CSV Name, flair args

            foreach (var x in patchSets) 
            {
                string templateFilePath =    @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\PatchMaker\{x.templateName}.xml~";
                string destinationFilePath = @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\{x.templateName}.xml";
                string csvFilePath =         @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\PatchMaker\{x.csvName}.csv";
                int flairArgs = x.flairArgNumber;

                if ((patchMode & PatchMode.Wipe) > PatchMode.None)
                {
                    string[] files = Directory.GetFiles(@$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\", "*.xml");
                    foreach (string file in files)
                        File.Delete(file);
                }

                if ((patchMode & PatchMode.Refresh) > PatchMode.None)
                    PatchMaker.MakePatchWithCSV(templateFilePath, destinationFilePath, csvFilePath, flairArgs);
            }


            
        }

        public struct PatchData
        {
            public PatchData(string templateName, string csvName, int flairArgNumber) 
            {
                this.templateName = templateName;
                this.csvName = csvName;
                this.flairArgNumber = flairArgNumber;
            }

            public string templateName;
            public string csvName;
            public int flairArgNumber;
        }

        [Flags]
        public enum PatchMode
        {
            None = 0,
            Wipe = 1,
            Refresh = 2,
            WipeAndRefresh = 3,
        }
    }
}
