using AlienRace;
using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(HarmonyPatches))]
    [HarmonyPatch(nameof(HarmonyPatches.BaseHeadOffsetAtHelper))]
    class HarmonyPatches_BaseHeadOffsetAtHelper_AddExtraBodySpecificHeadOffset
    {
        public static void Postfix(ref Vector2 __result, Pawn pawn) 
        {
            string raceName = GeneralUtility.GetRaceName(pawn);

            if (raceToHeadOffset.ContainsKey(raceName))
            {
                if (BodyTypeUtility.HasCustomBody(pawn)) 
                {
                    __result.x -= raceToHeadOffset[raceName].x;
                    __result.y -= raceToHeadOffset[raceName].y;
                }
            }

            return;
        }

        static Dictionary<string, Vector2> raceToHeadOffset = new Dictionary<string, Vector2> 
        {
            { "Alien_Equium",              new Vector2(0, -0.06f ) },
            { "Alien_KEquium",             new Vector2(0, -0.06f ) },
            { "Alien_PEquium",             new Vector2(0, -0.06f ) },
            { "Alien_SMaleEquium",         new Vector2(0, -0.06f ) },

            { "Alien_Orassan",    new Vector2(0, -0.06f )},
            { "AFoxbold",         new Vector2(0, -0.1f )},
            { "Alien_Dogbold",    new Vector2(0, -0.1f )},
            { "O21_FR_DarkElf",   new Vector2(0, -0.025f )},
            { "O21_FR_Gith",      new Vector2(0, 0.08f )},
            { "O21_FR_Dwarf",     new Vector2(0, -0.06f )},
            { "O21_FR_Goblin",    new Vector2(0, -0.1f )},
            { "O21_FR_Halfling",  new Vector2(0, -0.08f )},
            { "O21_FR_Kobold",    new Vector2(0, -0.1f )},
            { "O21_FR_WoodElf",   new Vector2(0, 0.05f )},
            { "O21_FR_MoonElf",   new Vector2(0, 0.05f )},
            { "O21_FR_SunElf",    new Vector2(0, 0.05f )},
        };

    }
}
