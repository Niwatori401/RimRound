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
    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch(nameof(PawnRenderer.BaseHeadOffsetAt))]
    class PawnRenderer_BaseHeadOffsetAt_AdjustForOptionalNorthCustomSprites
    {
        static void Postfix(Rot4 rotation, ref Vector3 __result, Pawn ___pawn) 
        {
			if ((GlobalSettings.alternateNorthHeadPositionForRRBodytypes && BodyTypeUtility.HasCustomBody(___pawn)) || Utilities.MobilityChairUtility.IsWearingAMobilityScooter(___pawn))
			switch (rotation.AsInt)
			{
				case 0:
					__result.y += -0.01f;
					return;
				case 1:
					return;
				case 2:
					return;
				case 3:
					return;
				default:
					Log.Error("BaseHeadOffsetAt Patch error!");
					return;
			}
		}
    }
}
