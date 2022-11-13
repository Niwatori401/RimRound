using AlienRace;
using AlienRace.ExtendedGraphics;
using HarmonyLib;
using Mono.Cecil.Cil;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(AlienPartGenerator))]
    [HarmonyPatch(nameof(AlienPartGenerator.GenerateMeshsAndMeshPools))]
    [HarmonyPatch(new Type[] { typeof(IGraphicsLoader) })]
    internal class AlienPartGenerator_GenerateMeshsAndMeshPools_MakeAllRRBodyTexRequestsUseDefaultPath
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codeInstructions.Count; i++)
            {
                if (codeInstructions[i].opcode == OpCodes.Ldstr && (string)codeInstructions[i].operand == "Naked_")
                {
                    Log.Message("ASS");
                    codeInstructions[i + 2] = new CodeInstruction(OpCodes.Call, ReplacementMethodInfo);
                }
            }

            return codeInstructions;
        }

        static string GetNewBodyString(BodyTypeDef bodyType)
        {
            Log.Message($"Write LINE {bodyType}");
            if (BodyTypeUtility.IsCustomBody(bodyType))
            {
                string bodytypeCleaned = RacialBodyTypeInfoUtility.GetEquivalentBodyTypeDef(bodyType).ToString();

                bodytypeCleaned = BodyTypeUtility.ConvertBodyTypeDefDefnameAccordingToSettings(bodytypeCleaned);

                Log.Message($"Changed {bodyType.defName} to {bodytypeCleaned}");


                return bodytypeCleaned;
            }

            return bodyType.defName;
        }

        static readonly MethodInfo ReplacementMethodInfo =
            typeof(AlienPartGenerator_GenerateMeshsAndMeshPools_MakeAllRRBodyTexRequestsUseDefaultPath)
            .GetMethod(nameof(AlienPartGenerator_GenerateMeshsAndMeshPools_MakeAllRRBodyTexRequestsUseDefaultPath.GetNewBodyString), BindingFlags.NonPublic | BindingFlags.Static);

    }
}
