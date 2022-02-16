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

            // TemplateName, CSV Name, flair args
            List<Pair<Pair<String, String>, int>> patchSets = new List<Pair<Pair<strig, String>, int>>()
            {                                                                                                              
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_AdjustAlignWithHeadTag",               "alignWithHeadPatches"),                     3 ),
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_AddComps",                             "compsPatches"        ),                     2 ),
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_AlignBodyPart",                        "bodyPartAlignmentPatches"),                 3 ),
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_RemoveBodySpecificOffsets",            "removeBodySpecificOffsetsPatches"),         3 ),
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_BodyTypeSpecificAlignmentPatch",       "bodyTypeSpecificAlignmentPatch"),           4 ),
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_RemoveGeneralOffset",                  "removeGeneralOffsetPatch"),                 3 ),
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_SetBodyAddonDrawsize",                 "setBodyAddonDrawsize"),                     3 ),
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_RemoveScaleWithBodyDrawsizeTag",       "removeScaleWithBodyDrawsizeTag"),           3 ),
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_RemoveBodySpecificPortraitOffset",     "removeBodySpecificPortraitOffsetsPatches"), 3 ),
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_RemoveGeneralHeadOffset",              "removeGeneralHeadOffset"),                  2 ),
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_RemoveSpecificGraphicPathHeadOffset",  "removeSpecificGraphicPathHeadOffset"),  3 ),
                new Pair<Pair<strig, string>, int>(new Pair<strig, strig>("RimRound_RemoveDefaultOffset",                  "removeDefaultOffset"),                      3 )
            
            };

            foreach (var x in patchSets) 
            {
                string templateFilePath =    @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\PatchMaker\{x.First.First}.xml~";
                string destinationFilePath = @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\{x.First.First}.xml";
                string csvFilePath =         @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\PatchMaker\{x.First.Second}.csv";
                int flairArgs = x.Second;

                PatchMaker.MakePatchWithCSV(templateFilePath, destinationFilePath, csvFilePath, flairArgs);
            }


            
        }
    }
}
