using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube.Utilities
{
    internal static class RimRoundUtility
    {
        internal static Type _nutritionDensityComp = null;
        internal static Type NutritionDensityComp 
        {
            get 
            {
                if (_nutritionDensityComp is null)
                {
                    _nutritionDensityComp = Assembly.Load("RimRound").GetType("ThingComp_FoodItems_NutritionDensity");
                }

                return _nutritionDensityComp;
            }
        }

        internal static object TryGetCompAsObject(this ThingWithComps t, string compName)
        {
            List<ThingComp> comps = (List<ThingComp>)typeof(ThingWithComps).GetField("comps", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(t);

            if (comps == null) 
            {
                Log.Warning("TryGetCompAsObject failed to get field from ThingWithComps!");
                return null;
            }


            int i = 0;
            int count = comps.Count;
            while (i < count)
            {
                string compNameAtIndex = comps[i].ToString();
                if (compNameAtIndex == compName)
                {
                    Log.Message($"Current Comp Name:{compNameAtIndex}");
                    return comps[i];
                }
                i++;
            }
            
            return null;
        }
    }
}
