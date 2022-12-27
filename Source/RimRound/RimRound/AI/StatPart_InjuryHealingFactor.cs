using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static RimRound.Utilities.Perks;

namespace RimRound.AI
{
    internal class StatPart_InjuryHealingFactor : StatPart
    {
        public override string ExplanationPart(StatRequest req)
        {
            if (req.Thing is Pawn pawn)
            {
                if (pawn.RaceProps.Humanlike)
                {
                    float val = 0;
                    FullnessAndDietStats_ThingComp comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
                    if (comp is null)
                        return "";

                    int rotundRegeneration = comp.perkLevels.PerkToLevels?["RR_RotundRegeneration_Title"] ?? 0;

                    if (!Utilities.BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_080_Gigantic))
                        rotundRegeneration = 0;

                    val += rotundRegeneration * 0.5f;

                    return "RR_RotundRegeneration_Title".Translate() + $" {(val * 100):f1}%";
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

                    int rotundRegeneration = comp.perkLevels.PerkToLevels?["RR_RotundRegeneration_Title"] ?? 0;

                    if (!Utilities.BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_080_Gigantic))
                        rotundRegeneration = 0;

                    val += rotundRegeneration * 0.5f;
                }
            }
        }
    }
}
