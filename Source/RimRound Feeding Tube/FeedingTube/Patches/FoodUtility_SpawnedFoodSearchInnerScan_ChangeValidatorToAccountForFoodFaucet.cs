using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimRound.FeedingTube.Patches
{
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch("SpawnedFoodSearchInnerScan")]
    public class FoodUtility_SpawnedFoodSearchInnerScan_ChangeValidatorToAccountForFoodFaucet
    {
        public static bool Prefix(Pawn __0, IntVec3 __1, List<Thing> __2, ref Predicate<Thing> __6) 
        {
             if (__6 is null)
            {
                Predicate<Thing> predicate = delegate (Thing t)
                {
                    Pawn getter = null;

                    if (__0.Position == __1)
                        getter = __0;

                    foreach (var x in __1.GetThingList(__0.Map))
                    {
                        if (x is Pawn p)
                        {
                            getter = p;
                            break;
                        }
                    }

                    if (getter is null)
                    {
                        Log.Error("Getter was null in SpawnedFoodSearchInnerScan patch!");
                    }

                    return FoodValidator(t, getter, __0, (getter.InMentalState || __0.InMentalState));
                };

                __6 = predicate;

                return true;
            }
            else 
            {
                Predicate<Thing> tempCopyPredicate = __6;
                Predicate<Thing> predicate = delegate (Thing t)
                {
                    Pawn getter = null;

                    if (__0.Position == __1)
                        getter = __0;

                    foreach (var x in __1.GetThingList(__0.Map))
                    {
                        if (x is Pawn p)
                        {
                            getter = p;
                            break;
                        }
                    }

                    if (getter is null)
                    {
                        Log.Error("Getter was null in SpawnedFoodSearchInnerScan patch!");
                    }

                    return t is Building_FoodFaucet ? FoodValidator(t, getter, __0, (getter.InMentalState || __0.InMentalState)) : tempCopyPredicate(t);
                };

                __6 = predicate;
                return true;
            }
        }


        public static bool FoodValidator(Thing t, Pawn getter, Pawn eater, bool allowForbidden)
        {
            if (t is Building_FoodFaucet building_FoodFaucet)
            {
                //!eater.WillEat(ThingDefOf.MealNutrientPaste, getter, true) ||
                //Defs.ThingDefOf.RR_FeedingTubeFluid.ingestible.preferability > FoodPreferability.MealLavish ||
                //Defs.ThingDefOf.RR_FeedingTubeFluid.ingestible.preferability < FoodPreferability.MealAwful ||
                if (
                getter != eater ||
                !(getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation)) ||
                (t.Faction != getter.Faction && t.Faction != getter.HostFaction) ||
                (!allowForbidden && t.IsForbidden(getter)) ||
                (!building_FoodFaucet.foodNetTrader.CanBeOn) ||
                !t.InteractionCell.Standable(t.Map) ||
                !getter.Map.reachability.CanReachNonLocal(
                    getter.Position,
                    new TargetInfo(t.InteractionCell, t.Map, false),
                    PathEndMode.OnCell,
                    TraverseParms.For(getter, Danger.Some, TraverseMode.ByPawn, false, false, false)) ||
                ((Building_FoodFaucet)t).foodNetTrader.FoodNet.Stored <= ((Building_FoodFaucet)t).litersPerDispense)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
