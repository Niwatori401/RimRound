using HarmonyLib;
using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(ITab_Pawn_Visitor))]
    [HarmonyPatch("FillTab")]
    class ITab_Pawn_Visitor_FillTab_AddLabelsForGainingReluctance
    {
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
			List<CodeInstruction> newInstructions = new List<CodeInstruction>();

			int startJndex = -1;
            bool foundInsertionPoint = false;


            for (int i = 0; i < codeInstructions.Count; ++i) 
			{
				if (codeInstructions[i].opcode == OpCodes.Ldstr && (string)codeInstructions[i].operand == "RecruitmentResistance") 
				{
                    startJndex = i;
				}
			}

			newInstructions.Add(new CodeInstruction(OpCodes.Dup));
			newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_0));
			newInstructions.Add(new CodeInstruction(OpCodes.Call, getSelPawnMI));
			newInstructions.Add(new CodeInstruction(OpCodes.Call, AddLabelsAboverecruitmentResistanceMI));
		


			if (startJndex != -1)
			{
                foundInsertionPoint = true;

                codeInstructions.InsertRange(startJndex, newInstructions);
			}

            if (!foundInsertionPoint)
                Log.Error($"Failed to find insertion point in {nameof(ITab_Pawn_Visitor_FillTab_AddLabelsForGainingReluctance)}.");
            
			return codeInstructions;
		}

		static MethodInfo getSelPawnMI = typeof(ITab).GetProperty("SelPawn", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
		static MethodInfo AddLabelsAboverecruitmentResistanceMI = typeof(ITab_Pawn_Visitor_FillTab_AddLabelsForGainingReluctance).GetMethod(nameof(ITab_Pawn_Visitor_FillTab_AddLabelsForGainingReluctance.AddLabelsAboverecruitmentResistance), BindingFlags.NonPublic | BindingFlags.Static);


		static void AddLabelsAboverecruitmentResistance(Listing_Standard ls, Pawn prisoner) 
		{
			var paComp = prisoner.TryGetComp<ThingComp_PawnAttitude>();
			if (paComp is null)
				return;

			Rect containingRect = ls.Label($"{ "GainingResistance".Translate()}: {paComp.GainingResistance:F1}" );
			if (Mouse.IsOver(containingRect)) 
			{
				TooltipHandler.TipRegion(containingRect, "GainingResistanceDesc".Translate());
			}
		}
	}
}
