using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using HarmonyLib;

//The world's jankiest patch \o7
namespace RimRound.Patch
{
#if DEBUG

	[HarmonyPatch(typeof(Pawn))]
	[HarmonyPatch("TickRare")]
	//Removes Cryptosleep sickness from newly spawned pawns because its annoying
	public class Pawn_RareTick_CryptoSleepSicknessRemoval
	{
		public static void Postfix(Pawn __instance)
		{
			if (__instance.RaceProps.Humanlike)
			{
				if (__instance.Position.x != -1000)
				{
					if (__instance.health?.hediffSet?.GetFirstHediffOfDef(HediffDef.Named("CryptosleepSickness"))?.TryGetComp<HediffComp_Disappears>().ticksToDisappear != null)
					{
						__instance.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("CryptosleepSickness")).TryGetComp<HediffComp_Disappears>().ticksToDisappear = 0;
					}
					else
					{
						return;
					}
				}
				else
				{
					return;
				}
			}
		}
	}

#endif
}
