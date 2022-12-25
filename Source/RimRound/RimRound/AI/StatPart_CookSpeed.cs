using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.AI
{
    internal class StatPart_CookSpeed : StatPart
    {
        public override string ExplanationPart(StatRequest req)
        {
            FullnessAndDietStats_ThingComp comp = req.Thing?.AsPawn()?.TryGetComp<FullnessAndDietStats_ThingComp>();

            int culinaryConniseurLevel = comp?.perkLevels?.PerkToLevels?["RR_Culinary_Connisseur_Title"] ?? 0;
            
            return $"Culinary Connoisseur: +{culinaryConniseurLevel * 1}";
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.Thing is Pawn pawn)
            {
                if (pawn.RaceProps.Humanlike)
                {
                    FullnessAndDietStats_ThingComp comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
                    if (comp is null)
                        return;

                    int culinaryConniseurLevel = comp.perkLevels.PerkToLevels?["RR_Culinary_Connisseur_Title"] ?? 0;

                    val += culinaryConniseurLevel * 1;
                }
            }
        }
    }
}
