using HarmonyLib;
using RimRound.Comps;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Caravan_NeedsTracker))]
    [HarmonyPatch("TrySatisfyFoodNeed")]
    public class Caravan_NeedsTracker_TrySatisfyFoodNeed_DontEatIfFullnessSatisfies
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            bool hookFound = false;

            MethodInfo curCategoryMI = typeof(Need_Food).GetProperty(nameof(Need_Food.CurCategory)).GetGetMethod(true);
            if (curCategoryMI is null)
                Log.Error($"curCategoryMI was null in {nameof(Caravan_NeedsTracker_TrySatisfyFoodNeed_DontEatIfFullnessSatisfies)}");


            MethodInfo replacementMI = typeof(Caravan_NeedsTracker_TrySatisfyFoodNeed_DontEatIfFullnessSatisfies)
                .GetMethod(nameof(Caravan_NeedsTracker_TrySatisfyFoodNeed_DontEatIfFullnessSatisfies.ReplacementMethod), BindingFlags.NonPublic | BindingFlags.Static);
            if (replacementMI is null)
                Log.Error($"replacementMI was null in {nameof(Caravan_NeedsTracker_TrySatisfyFoodNeed_DontEatIfFullnessSatisfies)}");

            for (int i = 0; i < codeInstructions.Count; i++)
            {
                if (!codeInstructions[i].Calls(curCategoryMI))
                    continue;

                hookFound = true;

                codeInstructions[i] = new CodeInstruction(OpCodes.Call, replacementMI);
                codeInstructions.Insert(i, new CodeInstruction(OpCodes.Ldarg_1)); //Pawn

                break;
            }


            if (!hookFound)
                Log.Error($"Failed to find insertion hook in {nameof(Caravan_NeedsTracker_TrySatisfyFoodNeed_DontEatIfFullnessSatisfies)}");

            return codeInstructions;
        }


        /// <summary>
        /// Prevents pawns from eating if they shouldn't due to fullness or nutrition
        /// </summary>
        /// <returns>HungerCategory.Fed if should not eat</returns>
        private static int ReplacementMethod(Need_Food need, Pawn pawn) 
        {
            HungerCategory curHunger = need.CurCategory;
            if (curHunger < HungerCategory.Hungry)
                return (int)HungerCategory.Fed;

            var comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (comp is null)
                return (int)curHunger;

            float currentNutritionWithFullnessPercentage = (need.CurLevel + (comp.CurrentFullness * comp.CurrentFullnessToNutritionRatio)) / need.MaxLevel;


            if (currentNutritionWithFullnessPercentage <= 0f)
            {
                return (int)HungerCategory.Starving;
            }
            if (currentNutritionWithFullnessPercentage < need.PercentageThreshUrgentlyHungry)
            {
                return (int)HungerCategory.UrgentlyHungry;
            }
            if (currentNutritionWithFullnessPercentage < need.PercentageThreshHungry)
            {
                return (int)HungerCategory.Hungry;
            }
            return (int)HungerCategory.Fed;
        }
    }
}
