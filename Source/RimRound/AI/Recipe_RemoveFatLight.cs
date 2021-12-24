using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.AI
{
    public class Recipe_RemoveFatLight : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            base.ApplyOnPawn(pawn, part, billDoer, ingredients, bill);

			if (billDoer != null)
			{
				if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
				{
					return;
				}
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
				{
					billDoer,
					pawn
				});
			}

			Utilities.HediffUtility.AddHediffSeverity(
				Defs.HediffDefOf.RimRound_Weight,
				pawn,
				-1 * (Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, pawn)?.Severity ?? 0) * Values.RandomFloat(variationPercent, 1.0f + variationPercent) * fatRemovedMultiplier);
		}

		private static float fatRemovedMultiplier = 0.2f;
		private static float variationPercent = 0.3f;
	}
}
