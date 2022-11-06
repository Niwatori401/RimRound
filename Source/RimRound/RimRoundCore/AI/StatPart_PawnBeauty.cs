using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.AI
{
    internal class StatPart_PawnBeauty : StatPart
    {
        public override string ExplanationPart(StatRequest req)
        {
            if (req.Thing is Pawn pawn)
            {
                if (pawn.RaceProps.Humanlike)
                {
                    FullnessAndDietStats_ThingComp comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
                    if (comp is null)
                        return "";

                    int beautifulBulk = comp.perkLevels.PerkToLevels?["RR_Beautiful_Bulk_Title"] ?? 0;

                    return "Bonus from Beautiful Bulk: " + beautifulBulk * 1f;
                }
            }

            return "";
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

                    int beautifulBulk = comp.perkLevels.PerkToLevels?["RR_Beautiful_Bulk_Title"] ?? 0;

                    val += beautifulBulk * 1f;
                }
            }
        }
    }
}
