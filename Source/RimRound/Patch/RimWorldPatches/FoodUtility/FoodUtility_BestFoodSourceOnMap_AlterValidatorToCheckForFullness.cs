using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
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
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch(nameof(FoodUtility.BestFoodSourceOnMap))]
    public class FoodUtility_BestFoodSourceOnMap_AlterValidatorToCheckForFullness
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) 
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newInstructions = new List<CodeInstruction>();

            int startJndex = -1;

            MethodInfo zigmaMI = typeof(FoodUtility_BestFoodSourceOnMap_AlterValidatorToCheckForFullness).GetMethod(nameof(Zigma), BindingFlags.Public | BindingFlags.Static);


            if (zigmaMI is null)
                Log.Error($"Error getting method info for {nameof(Zigma)}!");

            for (int jndex = 0; jndex < codeInstructions.Count; ++jndex) 
            {
                if (codeInstructions[jndex].opcode == OpCodes.Ldftn && codeInstructions[jndex + 1].opcode == OpCodes.Newobj) 
                {
                    startJndex = jndex + 2;
                    break;
                }
            }

            newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_1));
            newInstructions.Add(new CodeInstruction(OpCodes.Call, zigmaMI));



            if (startJndex != -1)
            {
                codeInstructions.InsertRange(startJndex, newInstructions);
            }

            return codeInstructions.AsEnumerable();
        }
        
        static bool FeedingTubeModInstalled() 
        {
            try
            {
                Activator.CreateInstance("RimRound Feeding Tube", "Building_FoodFaucet");
                return true;
            }
            catch (Exception) 
            {
                return false;
            }
        }

        public static Predicate<Thing> Zigma(Predicate<Thing> ogPredicate, Pawn eater)//, Pawn eater, ThingDef foodDef)
        {
            Predicate<Thing> dogma = delegate (Thing t)
            {
                //Type a = typeof(t);
                if (eater is null || !eater.RaceProps.Humanlike || t is null || !(eater.TryGetComp<FullnessAndDietStats_ThingComp>() is FullnessAndDietStats_ThingComp fullnessComp) || fullnessComp is null)
                {
                    if (!eater.RaceProps.Humanlike && t != null)
                    {
                        return ogPredicate(t) && !t.def.defName.Contains("FeedingTube") && !t.def.defName.Contains("FoodFaucet");
                    }
                     return ogPredicate(t);
                }

                float ftnRatio = FullnessAndDietStats_ThingComp.defaultFullnessToNutritionRatio;

                if (t.TryGetComp<ThingComp_FoodItems_NutritionDensity>() is ThingComp_FoodItems_NutritionDensity nutDensityComp && nutDensityComp != null)
                {
                    ftnRatio = nutDensityComp.Props.fullnessToNutritionRatio;
                }
                int stackCount = 0;
                float nutritionValueOfMeal = t.GetStatValue(StatDefOf.Nutrition, true) * ftnRatio;

                float wantedNutrition = fullnessComp.DietMode == DietMode.Fullness || fullnessComp.DietMode == DietMode.Hybrid ? fullnessComp.RemainingFullnessUntil(fullnessComp.GetRanges().Second) * ftnRatio : fullnessComp.RemainingFullnessUntil(fullnessComp.GetRanges().Second);



                stackCount = FoodUtility.StackCountForNutrition(wantedNutrition, nutritionValueOfMeal);

                if (!fullnessComp.SetAboveHardLimit && fullnessComp.RemainingFullnessUntil(fullnessComp.HardLimit) <=  nutritionValueOfMeal * stackCount)
                    return false;

                return ogPredicate(t);
            };

            return dogma;
        }

    }
}
