using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    /// <summary>
    /// This patch enabled hair to be drawn if a pawn is wearing a shell clothing item that normally covers their head and they have an RR body.
    /// </summary>
    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("ShellFullyCoversHead")]
    public class PawnRenderer_ShellFullyCoversHead_AlterShellClothingForRRBodies
    {
        static FieldInfo pawnFieldInfo = typeof(PawnRenderer).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void Postfix(ref bool __result, PawnRenderer __instance) 
        {
            if (!__result)
                return;

            Pawn pawn = (Pawn)pawnFieldInfo.GetValue(__instance);

            if (pawn is null)
            {
                Log.Error($"Pawn was null in {nameof(PawnRenderer_ShellFullyCoversHead_AlterShellClothingForRRBodies.Postfix)}");
                return;
            }

            if (BodyTypeUtility.HasCustomBody(pawn))
                __result = false;

            return;
        }
    }
}
