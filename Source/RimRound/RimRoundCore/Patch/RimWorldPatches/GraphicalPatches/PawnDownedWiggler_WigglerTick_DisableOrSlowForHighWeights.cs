using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using RimWorld;
using Verse;
using HarmonyLib;
using RimRound.Utilities;

namespace RimRound.Patch
{
	[HarmonyPatch(typeof(PawnDownedWiggler))]
	[HarmonyPatch("WigglerTick")]
	public class PawnDownedWiggler_WigglerTick_DisableOrSlowForHighWeights
    {
		public static bool Prefix(PawnDownedWiggler __instance, Pawn ___pawn) 
        {
			if (!(___pawn?.RaceProps?.Humanlike ?? true) || !___pawn.Downed || !___pawn.Spawned || ___pawn.InBed())
				return true;

			BodyTypeInfo? bodyTypeInfo = RacialBodyTypeInfoUtility.GetRacialBodyTypeInfo(___pawn);
			if (bodyTypeInfo is null)
				return true;

			float wiggleSpeedFactor = bodyTypeInfo.AsNonNullable().wiggleSpeed;

			__instance.ticksToIncapIcon--;
			if (__instance.ticksToIncapIcon <= 0)
			{
				FleckMaker.ThrowMetaIcon(___pawn.Position, ___pawn.Map, FleckDefOf.IncapIcon, 0.42f);
				__instance.ticksToIncapIcon = 200;
			}

			if (___pawn.Awake())
			{
				int num = Find.TickManager.TicksGame % 300 * 2;
				if (num < 90)
				{
					__instance.downedAngle += 0.35f* wiggleSpeedFactor;
				}
				if (num < 390 && num >= 300)
				{
					__instance.downedAngle -= 0.35f * wiggleSpeedFactor;
				}
			}

			return false;
        }
    }
}
