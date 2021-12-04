using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Comps
{
    public class ThingComp_FoodItems_NutritionDensity : ThingComp
    {
        public CompProperties_FoodItems_NutritionDensity Props 
        {
            get 
            {
                return this.props as CompProperties_FoodItems_NutritionDensity;
            }
        }
    }
}
