using RimRound.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Utilities
{
    public class HungerDroneUtility
    {
        public static float GetCurrentHungerMultiplierFromDrone(Pawn p) 
        {
            HungerDroneLevel hungerDroneLevel = GetHighestHungerDroneLevelFor(p);

            switch (hungerDroneLevel)
            {
                case HungerDroneLevel.None:
                    return 1;
                case HungerDroneLevel.Low:
                    return 1.5f;
                case HungerDroneLevel.Medium:
                    return 2.0f;
                case HungerDroneLevel.Severe:
                    return 3.0f;
                default:
                    throw new NotImplementedException();
            }
        }

        public static float GetCurrentNutritionFallMultiplierFromDrone(Pawn p)
        {
            HungerDroneLevel hungerDroneLevel = GetHighestHungerDroneLevelFor(p);

            switch (hungerDroneLevel)
            {
                case HungerDroneLevel.None:
                    return 1;
                case HungerDroneLevel.Low:
                    return 1.5f;
                case HungerDroneLevel.Medium:
                    return 2.0f;
                case HungerDroneLevel.Severe:
                    return 3.0f;
                default:
                    throw new NotImplementedException();
            }
        }

        static HungerDroneLevel GetHighestHungerDroneLevelFor(Pawn p) 
        {
            HungerDroneLevel hungerDroneLevel = HungerDroneLevel.None;
            if (!p.RaceProps.Humanlike)
                return HungerDroneLevel.None;

            var activeConditions = p.Map.gameConditionManager.ActiveConditions;
            for (int i = 0; i < activeConditions.Count; i++)
            {
                if (activeConditions[i] is GameCondition_PsychicHungerIncrease gc)
                {
                    if (gc.gender == p.gender && gc.level > hungerDroneLevel)
                    {
                        hungerDroneLevel = gc.level;
                    }
                }
            }

            return hungerDroneLevel;
        }

        public enum HungerDroneLevel
        {
            None,
            Low,
            Medium,
            Severe
        }
    }
}
