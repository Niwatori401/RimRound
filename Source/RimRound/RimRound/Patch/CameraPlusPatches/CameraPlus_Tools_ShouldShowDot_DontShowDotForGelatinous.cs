using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    public class CameraPlus_Tools_ShouldShowDot_DontShowDotForGelatinous
    {
        public static void Postfix(Pawn pawn, ref bool __result) 
        {
            if (pawn?.story?.bodyType?.defName?.Contains("Gelatinous") ?? false)
            {
                __result = false;
            }
        }


        public static PatchCollection GetPatchCollection()
        {
            return new PatchCollection
            {
                postfix = typeof(CameraPlus_Tools_ShouldShowDot_DontShowDotForGelatinous).GetMethod(
                    nameof(CameraPlus_Tools_ShouldShowDot_DontShowDotForGelatinous.Postfix), ModCompatibilityUtility.majorFlags)
            };
        }
    }
}
