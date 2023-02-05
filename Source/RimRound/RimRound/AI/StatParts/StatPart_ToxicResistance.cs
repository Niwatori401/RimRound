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
    public class StatPart_ToxicResistance : StatPart
    {
        public override string ExplanationPart(StatRequest req)
        {
            if (!(req.Thing is Pawn pawn) || !pawn.RaceProps.Humanlike)
                return "";

            FullnessAndDietStats_ThingComp comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (comp is null)
                return "";

            int thatLevel = comp.perkLevels.PerkToLevels?["RR_That_Title"] ?? 0;

            if (!BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_500_Gelatinous))
                thatLevel = 0;

            return "RR_That_Title".Translate() + $" {(thatLevel * 100):f1}%";
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!(req.Thing is Pawn pawn) || !pawn.RaceProps.Humanlike)
                return;

            FullnessAndDietStats_ThingComp comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (comp is null)
                return;

            int thatLevel = comp.perkLevels.PerkToLevels?["RR_That_Title"] ?? 0;

            if (!BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_500_Gelatinous))
                thatLevel = 0;

            val += thatLevel;
        }
    }
}
