using HarmonyLib;
using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(StatWorker))]
    [HarmonyPatch(nameof(StatWorker.FinalizeValue))]
    public class StatWorker_FinalizeValue_AlterMaxValueForPerks
    {
        static MethodInfo clampMI = typeof(Mathf).GetMethod(nameof(Mathf.Clamp), BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder, new Type[] { typeof(float), typeof(float), typeof(float) }, null);
        static MethodInfo replacementClampMI = typeof(StatWorker_FinalizeValue_AlterMaxValueForPerks).GetMethod(nameof(ReplacementClamp), BindingFlags.Static | BindingFlags.NonPublic);
        
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool success = false;

            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

            if (clampMI is null)
            {
                Log.Error("Failed to get method info for Clamp method.");
                return instructions;
            }

            if (replacementClampMI is null)
            {
                Log.Error("Failed to get method info for replacement clamp method.");
                return instructions;
            }


            for (int i = 0; i < codeInstructions.Count; ++i)
            {
                if (!codeInstructions[i].Calls(clampMI))
                    continue;

                codeInstructions[i].operand = replacementClampMI;
                codeInstructions[i - 1] = new CodeInstruction(OpCodes.Ldarg_1);

                success = true;
                break;
            }

            if (!success)
                Log.Error("Failed to find suitable location in method StatWorker.FinalizeValue to patch with transpiler.");

            return codeInstructions;
        }

        static float ReplacementClamp(float val, float min, StatDef stat, StatRequest req) 
        {
            float defaultValue = Mathf.Clamp(val, min, stat.maxValue);

            if (!(req.Thing is Pawn pawn) || !pawn.RaceProps.Humanlike)
                return defaultValue;

            FullnessAndDietStats_ThingComp comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (comp is null)
                return defaultValue;


            float bonusToMax = 0;

            if (stat.defName == "ButcheryFleshEfficiency")
            {
                int theresTheBeefLevel = comp.perkLevels.PerkToLevels?["RR_TheresTheBeef_Title"] ?? 0; // One possible level, total of 50%

                bonusToMax = theresTheBeefLevel * 0.50f;
            }
            else if (stat.defName == "PlantHarvestYield")
            {
                int cropNotchLevel = comp.perkLevels.PerkToLevels?["RR_CropNotch_Title"] ?? 0; // Five possible levels, total of 50%

                bonusToMax = cropNotchLevel * 0.10f;
            }

            return Mathf.Clamp(val, min, stat.maxValue + bonusToMax);
        } 
    }
}
