using HarmonyLib;
using RimRound.Comps;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimRound.Patch
{
	/// <summary>
	/// This patch will cause wardens to stock the jail cell with food based on the prisoners' diet management settings rather than raw nutrition values IFF the interaction mode is set to RR_Fatten.
	/// </summary>

    [HarmonyPatch(typeof(WorkGiver_Warden_DeliverFood))]
    [HarmonyPatch(nameof(WorkGiver_Warden_DeliverFood.JobOnThing))]
    public class WorkGiver_Warden_DeliverFood_JobOnThing_AddExceptionForFattenInteraction
    {
        public static bool Prefix(Pawn __0, Thing __1, bool __2, ref Job __result, WorkGiver_Warden_DeliverFood __instance) 
        {
            Pawn warden = __0;
            Pawn prisoner = (Pawn)__1;
            bool isForced = __2;


			if (!(prisoner != null && 
				prisoner.IsPrisonerOfColony && 
				prisoner.guest.PrisonerIsSecure && 
				prisoner.Spawned && 
				!prisoner.InAggroMentalState && 
				!prisoner.IsForbidden(warden) && 
				!prisoner.IsFormingCaravan() && 
				warden.CanReserveAndReach(prisoner, PathEndMode.OnCell, warden.NormalMaxDanger(), 1, -1, null, isForced)))
			{
				return true;
			}

			if (prisoner.guest.interactionMode.defName != Defs.PrisonerInteractionModeDefOf.RR_Fatten.defName)
				return true;
 

			if (!prisoner.guest.CanBeBroughtFood)
			{
				return true;
			}
			if ((bool)FoodAvailableInRoomToMI.Invoke(null, new object[] { prisoner }))
			{
				return true;
			}


			if (!prisoner.Position.IsInPrisonCell(prisoner.Map))
			{
				return true;
			}

			if (WardenFeedUtility.ShouldBeFed(prisoner))
			{
				return true;
			}





			if (PrisonerShouldBeBroughtFood(prisoner)) 
			{

				__result = MakeBringPrisonerFoodJob(warden, prisoner);
				
				if (__result is null)
					return true;

				return false;
			}

			return true;
        }

		static MethodInfo FoodAvailableInRoomToMI = typeof(WorkGiver_Warden_DeliverFood).GetMethod("FoodAvailableInRoomTo", BindingFlags.NonPublic | BindingFlags.Static);

		static Job MakeBringPrisonerFoodJob(Pawn warden, Pawn prisoner) 
		{
			Thing thing;
			ThingDef thingDef;
			if (!FoodUtility.TryFindBestFoodSourceFor(warden, prisoner, prisoner.needs.food.CurCategory == HungerCategory.Starving, out thing, out thingDef, false, true, false, false, false, false, false, false, false, true, FoodPreferability.Undefined))
			{
				return null;
			}

			float nutrition = FoodUtility.GetNutrition(prisoner, thing, thingDef);
			Job job = JobMaker.MakeJob(JobDefOf.DeliverFood, thing, prisoner);
			job.count = FoodUtility.WillIngestStackCountOf(prisoner, thingDef, nutrition);
			job.targetC = RCellFinder.SpotToChewStandingNear(prisoner, thing, null);

			return job;
		}

		static bool PrisonerShouldBeBroughtFood(Pawn prisoner)
		{
			var fndComp = prisoner?.TryGetComp<FullnessAndDietStats_ThingComp>();

			if (fndComp is null)
				return false;

			switch (fndComp.DietMode)
			{
				case DietMode.Nutrition:
					if (prisoner.needs.food.CurLevel + fndComp.CurrentFullness / fndComp.CurrentFullnessToNutritionRatio <= fndComp.GetRanges().First)
						return true;
					else
						return false;

				case DietMode.Hybrid:
					if (prisoner.needs.food.CurLevel + fndComp.CurrentFullness / fndComp.CurrentFullnessToNutritionRatio <= fndComp.GetRanges().First)
						return true;
					else
						return false;
				case DietMode.Fullness:
					if (fndComp.CurrentFullness <= fndComp.GetRanges().First)
						return true;
					else
						return false;
				case DietMode.Disabled:
					return false;
				default:
					Log.Error($"{prisoner.Name.ToStringShort}'s DietMode was invalid in FeedPatientUtility_IsHungry_CheckForFullness");
					return false;

			}

		}
    }
}
