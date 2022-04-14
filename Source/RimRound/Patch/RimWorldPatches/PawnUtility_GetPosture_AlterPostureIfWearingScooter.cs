using HarmonyLib;
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

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnUtility))]
    [HarmonyPatch(nameof(PawnUtility.GetPosture))]
    class PawnUtility_GetPosture_AlterPostureIfWearingScooter
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) 
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newInstructions = new List<CodeInstruction>();


            Label breakToRestOfMethodLabel = generator.DefineLabel();
            codeInstructions[0].labels.Add(breakToRestOfMethodLabel);

            newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_0));
            newInstructions.Add(new CodeInstruction(OpCodes.Call, isWearingMobilityChairMI));
            newInstructions.Add(new CodeInstruction(OpCodes.Brfalse_S, breakToRestOfMethodLabel));
            newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_0));
            newInstructions.Add(new CodeInstruction(OpCodes.Call, getAppropriatePostureGivenWearingChairMI));
            newInstructions.Add(new CodeInstruction(OpCodes.Ret));

            codeInstructions.InsertRange(0, newInstructions);

            return codeInstructions;
        }

        static MethodInfo isWearingMobilityChairMI = typeof(MobilityChairUtility).GetMethod(nameof(MobilityChairUtility.IsWearingAMobilityScooter), BindingFlags.Public | BindingFlags.Static);
        static MethodInfo getAppropriatePostureGivenWearingChairMI = typeof(MobilityChairUtility).GetMethod(nameof(MobilityChairUtility.GetAppropriatePostureGivenWearingChair), BindingFlags.Public | BindingFlags.Static);


    }
}
