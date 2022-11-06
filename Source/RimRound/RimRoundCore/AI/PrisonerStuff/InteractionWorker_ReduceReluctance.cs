using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.AI
{
    public class InteractionWorker_ReduceReluctance : InteractionWorker
    {
        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
        {
			letterText = null;
			letterLabel = null;
			letterDef = null;
			lookTargets = null;

			ThingComp_PawnAttitude pawnAttitude = null;
			if (recipient?.TryGetComp<ThingComp_PawnAttitude>() is ThingComp_PawnAttitude comp)
			{
				pawnAttitude = comp;
			}
			else 
			{
				Log.Warning($"Ran interaction worker InteractionWorker_ReduceReluctance for a recipient {recipient.Name} and initiator {initiator.Name} without a ThingComp_PawnAttitude");
				return;
			}


			bool isAnimalOrWildman = recipient.AnimalOrWildMan();
			float opinionOfInitiator = (float)((recipient.relations != null) ? recipient.relations.OpinionOf(initiator) : 0);
			
			float resistanceReduce = 0f;

			if (!isAnimalOrWildman)
			{
				if (pawnAttitude.GainingResistance > 0f && Values.RandomChanceAtOrBelow(0.25f))
				{
					float amountToReduceBy = 1f;
					amountToReduceBy *= initiator.GetStatValue(StatDefOf.NegotiationAbility, true);
					amountToReduceBy *= InteractionWorker_ReduceReluctance.ResistanceImpactFactorCurve_Mood.Evaluate((recipient.needs.mood == null) ? 1f : recipient.needs.mood.CurInstantLevelPercentage);
					amountToReduceBy *= InteractionWorker_ReduceReluctance.ResistanceImpactFactorCurve_Opinion.Evaluate(opinionOfInitiator);
					amountToReduceBy = Mathf.Min(amountToReduceBy, pawnAttitude.GainingResistance);

					float resistance = pawnAttitude.GainingResistance;
					pawnAttitude.GainingResistance = Mathf.Max(0f, resistance - amountToReduceBy);
					resistanceReduce = resistance - recipient.guest.resistance;
					string text = "TextMote_GainingResistanceChange".Translate(resistance.ToString("F1"), pawnAttitude.GainingResistance.ToString("F1"));
					if (recipient.needs.mood != null && recipient.needs.mood.CurLevelPercentage < 0.4f)
					{
						text += "\n(" + "lowMood".Translate() + ")";
					}
					if (recipient.relations != null && (float)recipient.relations.OpinionOf(initiator) < -0.01f)
					{
						text += "\n(" + "lowOpinion".Translate() + ")";
					}
					MoteMaker.ThrowText((initiator.DrawPos + recipient.DrawPos) / 2f, initiator.Map, text, 8f);
				}

				AdjustDietBarsBasedOnResistance(recipient);

				recipient.guest.SetRecruitmentData(initiator);
				return;
			}
		}


		private static void AdjustDietBarsBasedOnResistance(Pawn pawn) 
		{
			var fndcomp = pawn?.TryGetComp<FullnessAndDietStats_ThingComp>();
			var paComp = pawn?.TryGetComp<ThingComp_PawnAttitude>();

			if (fndcomp is null || paComp is null || fndcomp.DietMode == DietMode.Disabled)
				return;

			float gainingResistance = paComp.GainingResistance;

			if (gainingResistance <= 3)
			{
				fndcomp.DietMode = DietMode.Fullness;
				fndcomp.SetRangesPercent(0.60f, 0.95f);
			} 
			else if (gainingResistance <= 10) 
			{
				fndcomp.DietMode = DietMode.Fullness;
				fndcomp.SetRangesPercent(0.01f, 0.80f);
			} 
			else if (gainingResistance <= 15) 
			{
				fndcomp.DietMode = DietMode.Fullness;
				fndcomp.SetRangesPercent(0.1f, 0.70f);
			} 			
			else if (gainingResistance <= 25) 
			{
				fndcomp.DietMode = DietMode.Nutrition;
				fndcomp.SetRangesPercent(0.30f, 0.90f);
			} 			
			else if (gainingResistance <= 35) 
			{
				fndcomp.DietMode = DietMode.Nutrition;
				fndcomp.SetRangesPercent(0.10f, 0.50f);
			} 
			else
			{
				fndcomp.DietMode = DietMode.Nutrition;
				fndcomp.SetRangesPercent(0.01f, 0.20f);
			}

			return;
		}

		private static readonly SimpleCurve ResistanceImpactFactorCurve_Mood = new SimpleCurve
		{
			{
				new CurvePoint(0f, 0.2f),
				true
			},
			{
				new CurvePoint(0.5f, 1f),
				true
			},
			{
				new CurvePoint(1f, 1.5f),
				true
			}
		};

		private static readonly SimpleCurve ResistanceImpactFactorCurve_Opinion = new SimpleCurve
		{
			{
				new CurvePoint(-100f, 0.5f),
				true
			},
			{
				new CurvePoint(0f, 1f),
				true
			},
			{
				new CurvePoint(100f, 1.5f),
				true
			}
		};
	}
}
