using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
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

                    return Functions.FoodValidator(t, getter, __0, (getter.InMentalState || __0.InMentalState));
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

                    return t is Building_FoodFaucet ? Functions.FoodValidator(t, getter, __0, (getter.InMentalState || __0.InMentalState)) : tempCopyPredicate(t);
                };

                __6 = predicate;
                return true;
            }
        }
    }
}
