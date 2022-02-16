using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace RimRound.AI
{
    public class WorkGiver_StuffPrisoner : WorkGiver_Warden_Feed
    {
        public override Job JobOnThing(Pawn feeder, Thing t, bool forced = false)
        {
			//if (t?.AsPawn()?.guest?.interactionMode is null || (t.AsPawn().guest.interactionMode != Defs.PrisonerInteractionModeDefOf.RR_Fatten && t.AsPawn().guest.interactionMode != Defs.PrisonerInteractionModeDefOf.RR_FattenForced))
			//{
			//	return null;
			//}

			Pawn feedee = t.AsPawn();

			if (feedee is null || !PossibleToBeFed(feeder, t, false) || !PrisonerShouldBeStuffed(feedee, prisonerShouldBeFedThresholdPercent))
			{
				return null;
			}

			Thing thing;
			ThingDef thingDef;
			#pragma warning disable CS0612 // Type or member is obsolete
            if (!FoodUtility.TryFindBestFoodSourceFor(feeder, feedee, feedee.needs.food.CurCategory == HungerCategory.Starving, out thing, out thingDef, false, true, false, false, false, false, false, false, false, FoodPreferability.Undefined))
			#pragma warning restore CS0612 // Type or member is obsolete
            {
				JobFailReason.Is("NoFood".Translate(), null);
				return null;
			}

			FullnessAndDietStats_ThingComp feedeeFnDComp = feedee.TryGetComp<FullnessAndDietStats_ThingComp>();

			feedeeFnDComp.DietMode = DietMode.Fullness;
			feedeeFnDComp.SetRanges(
				feedeeFnDComp.fullnessbar.CurrentFullnessAsPercentOfWhole + 0.01f, (prisonerShouldBeFedMaxPercent * feedeeFnDComp.fullnessbar.HardLimitAsPercentage)
				); 
			float nutrition = FoodUtility.GetNutrition(thing, thingDef);
			Job job = JobMaker.MakeJob(Defs.JobDefOf.RR_JD_StuffPrisoner, thing, feedee);
			job.count = FoodUtility.WillIngestStackCountOf(feedee, thingDef, nutrition);

			feedeeFnDComp.DietMode = DietMode.Disabled;
			return job;
		}

		protected bool PossibleToBeFed(Pawn warden, Thing prisoner, bool forced = false)
		{
            return 
				prisoner is Pawn pawn && 
				pawn.IsPrisonerOfColony && 
				pawn.guest.PrisonerIsSecure && 
				pawn.Spawned && 
				!pawn.InAggroMentalState && 
				!prisoner.IsForbidden(warden) && 
				!pawn.IsFormingCaravan() && 
				warden.CanReserveAndReach(pawn, PathEndMode.OnCell, warden.NormalMaxDanger(), 1, -1, null, forced);
        }

		private bool PrisonerShouldBeStuffed(Pawn p, float prisonerFeedThreshold)
		{
			return
				p.IsPrisonerOfColony &&
				p.guest.CanBeBroughtFood &&
				p.TryGetComp<FullnessAndDietStats_ThingComp>() is FullnessAndDietStats_ThingComp comp &&
				comp.CurrentFullness <= prisonerFeedThreshold;
			;
		}


		float prisonerShouldBeFedThresholdPercent = 0.50f;
		float prisonerShouldBeFedMaxPercent = 0.99f;
	}
}
