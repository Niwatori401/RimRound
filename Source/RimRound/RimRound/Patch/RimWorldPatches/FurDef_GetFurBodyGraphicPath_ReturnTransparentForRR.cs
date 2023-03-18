using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(FurDef))]
    [HarmonyPatch(nameof(FurDef.GetFurBodyGraphicPath))]
    public class FurDef_GetFurBodyGraphicPath_ReturnTransparentForRR
    {
        public static void Postfix(ref string __result, Pawn pawn) 
        {
            if (BodyTypeUtility.HasCustomBody(pawn))
                __result = "BlankTexture";

            return;
        }
    }
}
