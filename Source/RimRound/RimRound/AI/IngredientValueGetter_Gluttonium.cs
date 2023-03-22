using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.AI
{
    internal class IngredientValueGetter_Gluttonium : IngredientValueGetter
    {
        public override string BillRequirementsDescription(RecipeDef r, IngredientCount ing)
        {
            return ing.GetBaseCount() + "x " + "BillNutrition".Translate() + " (" + ing.filter.Summary + ")";
        }

        public override float ValuePerUnitOf(ThingDef t)
        {
            if (t == Defs.ThingDefOf.RR_Gluttonium)
                return 1f;

            if (!t.IsNutritionGivingIngestible)
                return 0f;

            return t.GetStatValueAbstract(StatDefOf.Nutrition, null);
        }
    }
}
