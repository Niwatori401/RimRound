using AlienRace;
using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RimRound.Patch
{
    //[HarmonyPatch(typeof(AlienPartGenerator))]
    //[HarmonyPatch(nameof(AlienPartGenerator.GetPath))]
    internal class AlienPartGenerator_GetNakedPath_MakeAllRRBodyTexRequestsUseDefaultPath
    {
        
        public static void Postfix(BodyTypeDef bodyType, string userpath, string gender, ref string __result) 
        {
            if (BodyTypeUtility.IsCustomBody(bodyType))
            {
                string bodytypeCleaned = RacialBodyTypeInfoUtility.GetEquivalentBodyTypeDef(bodyType).ToString();

                bodytypeCleaned = BodyTypeUtility.ConvertBodyTypeDefDefnameAccordingToSettings(bodytypeCleaned);

                __result = $"Things/Pawn/Humanlike/Bodies/Naked_{bodytypeCleaned}";
            }
        }


    }
}
