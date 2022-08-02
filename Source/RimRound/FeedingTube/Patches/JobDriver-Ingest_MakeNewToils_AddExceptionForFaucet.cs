using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using Verse.AI;
using System.Reflection;
using RimRound.Comps;
using RimRound.Utilities;
using UnityEngine;

namespace RimRound.FeedingTube.Patches
{
    [HarmonyPatch(typeof(JobDriver_Ingest))]
    [HarmonyPatch("MakeNewToils")]
    public class JobDriver_Ingest_MakeNewToils_AddExceptionForFaucet
    {
        static MethodInfo ChewDurationMultiplier = typeof(JobDriver_Ingest).GetProperty("ChewDurationMultiplier", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod(true);
        static MethodInfo IngestibleSourceGetter = typeof(JobDriver_Ingest).GetProperty("IngestibleSource", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod(true);
        static MethodInfo PrepareToIngestToils = typeof(JobDriver_Ingest).GetMethod("PrepareToIngestToils", BindingFlags.Instance | BindingFlags.NonPublic);


        public static bool Prefix(JobDriver_Ingest __instance, ref IEnumerable<Toil> __result) 
        {
            if ((__instance.job.GetTarget(TargetIndex.A).Thing) is Building_FoodFaucet foodFaucet) 
            {
                List<Toil> toilsToReturn = new List<Toil>();
                Thing ingestibleSource = (Thing)(IngestibleSourceGetter.Invoke(__instance, null));
                float chewDurationMultiplier = (float)ChewDurationMultiplier.Invoke(__instance, null);

                chewDurationMultiplier = 0.1f;

                Toil chew = Toils_Ingest.ChewIngestible(
                    __instance.pawn,
                    chewDurationMultiplier,
                    TargetIndex.B, 
                    TargetIndex.None).FailOn(
                        (Toil x) => !ingestibleSource.Spawned && 
                        (__instance.pawn.carryTracker == null ||
                        __instance.pawn.carryTracker.CarriedThing != ingestibleSource)).FailOnCannotTouch(TargetIndex.B, PathEndMode.Touch);

                if (__instance.pawn is null)
                    Log.Warning("Pawn instance was null :(");

                int numberOfMealsToGet = GetNumberOfTimesToEatFromSystem(__instance.pawn, foodFaucet);

                for (int i = 0; i < numberOfMealsToGet; i++)
                {
                    foreach (Toil toil in (IEnumerable<Toil>)PrepareToIngestToils.Invoke(__instance, new object[] { chew }))
                    {
                        Thing test5A = __instance.job.GetTarget(TargetIndex.A).Thing;
                        Thing test5B = __instance.job.GetTarget(TargetIndex.B).Thing;
                        toilsToReturn.Add(toil);
                        Thing test6A = __instance.job.GetTarget(TargetIndex.A).Thing;
                        Thing test6B = __instance.job.GetTarget(TargetIndex.B).Thing;
                    }

                    toilsToReturn.Add(chew);
                    toilsToReturn.Add(Toils_Ingest.FinalizeIngest(__instance.pawn, TargetIndex.B));
                    toilsToReturn.Add(Toils_Jump.JumpIf(chew, () => __instance.job.GetTarget(TargetIndex.B).Thing is Corpse && __instance.pawn.needs.food.CurLevelPercentage < 0.9f));
                }

                __result = toilsToReturn;

                return false;
            }
            return true;
        }


        static int GetNumberOfTimesToEatFromSystem(Pawn pawn, Building_FoodFaucet foodFaucet) 
        {
            float nutritionWanted;
            float nutritionPerDispense = foodFaucet.nutritionPerDispense;
            float netFullnessToNutritionRatio = foodFaucet.foodNetTrader.FoodNet.fullnessToNutritionRatio;
            FullnessAndDietStats_ThingComp fullnessComp = pawn?.TryGetComp<FullnessAndDietStats_ThingComp>();

            if (fullnessComp == null)
                return 0;
            if (!fullnessComp.parent.AsPawn().RaceProps.Humanlike)
                return 0;

            float burstingNutritionMult = 2f;

            switch (fullnessComp.DietMode)
            {
                case DietMode.Nutrition:
                    nutritionWanted = fullnessComp.SetAboveHardLimit ? (fullnessComp.GetRanges().Second - fullnessComp.GetRanges().First) * burstingNutritionMult : fullnessComp.GetRanges().Second - fullnessComp.GetRanges().First;
                    return (int)Mathf.Floor(nutritionWanted / nutritionPerDispense);
                case DietMode.Hybrid:
                    nutritionWanted = (fullnessComp.SetAboveHardLimit ? fullnessComp.RemainingFullnessUntil(fullnessComp.HardLimit) * burstingNutritionMult : (fullnessComp.GetRanges().Second - fullnessComp.CurrentFullness)) / netFullnessToNutritionRatio;// / fullnessComp.CurrentFullnessToNutritionRatio;
                    return (int)Mathf.Floor(nutritionWanted / nutritionPerDispense);
                case DietMode.Fullness:
                    nutritionWanted = (fullnessComp.SetAboveHardLimit ? fullnessComp.RemainingFullnessUntil(fullnessComp.HardLimit) * burstingNutritionMult : (fullnessComp.GetRanges().Second - fullnessComp.CurrentFullness)) / netFullnessToNutritionRatio;
                    return (int)Mathf.Floor(nutritionWanted / nutritionPerDispense);
                case DietMode.Disabled:
                    nutritionWanted = pawn.needs.food.MaxLevel - pawn.needs.food.CurLevel;
                    return (int)Mathf.Floor(nutritionWanted / nutritionPerDispense);
                default:
                    Log.Error($"{fullnessComp.parent.AsPawn().Name.ToStringShort}'s DietMode was invalid in RimRound_NeedFood_NutritionWantedPatch");
                    return 0;

            }
        }
    }
}
