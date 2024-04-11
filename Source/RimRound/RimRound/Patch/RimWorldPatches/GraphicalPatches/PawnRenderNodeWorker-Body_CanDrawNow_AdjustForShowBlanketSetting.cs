using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch.RimWorldPatches.GraphicalPatches
{
    [HarmonyPatch(typeof(PawnRenderNodeWorker_Body))]
    [HarmonyPatch(nameof(PawnRenderNodeWorker_Body.CanDrawNow))]
    public class PawnRenderNodeWorker_Body_CanDrawNow_AdjustForShowBlanketSetting
    {
        static Dictionary<string, HideCovers_ThingComp> pawnIdToComp = new Dictionary<string, HideCovers_ThingComp>();


        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

            UpdateCanDrawBasedOnSettings(codeInstructions);

            return codeInstructions;
        }

        private static void UpdateCanDrawBasedOnSettings(List<CodeInstruction> codeInstructions) {

            codeInstructions.RemoveRange(codeInstructions.Count() - 7, 4);
            codeInstructions.Insert(codeInstructions.Count() - 3, new CodeInstruction(OpCodes.Call, replacementMethodInfo));

        }

        
        static MethodInfo replacementMethodInfo = typeof(PawnRenderNodeWorker_Body_CanDrawNow_AdjustForShowBlanketSetting).GetMethod(nameof(ReplacementFunction), BindingFlags.Static | BindingFlags.NonPublic);

        // True means the body can draw
        private static bool ReplacementFunction(PawnDrawParms parms) {
            Pawn pawn = parms.pawn;

            HideCovers_ThingComp comp;

            if (!pawnIdToComp.TryGetValue(pawn.ThingID, out comp))
            {
                comp = pawn.TryGetComp<HideCovers_ThingComp>();
                pawnIdToComp.Add(pawn.ThingID, comp);
            }

            if (comp is null)
                return parms.bed.def.building.bed_showSleeperBody;



            if (parms.bed.IsBlobBed())
            {
                return true;
            }

            return comp.HideCovers;
        }

    }
}
