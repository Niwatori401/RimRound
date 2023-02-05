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
    public class StatPart_SkinYieldBodyFat : StatPart
    {
        public override string ExplanationPart(StatRequest req)
        {
            if (req.Thing is Pawn p && p.RaceProps.Humanlike)
            {
                float mult = 1 + ((GetSkinMultByWeight(req) - 1) * GlobalSettings.meatMultiplierForWeight.threshold);

                return "StatPart_MeatYieldBodyFatExplanation".Translate(mult.ToString("F2")) + ": x" + mult.ToStringPercent();
            }

            return null;
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.HasThing)
            {
                if (req.Thing is Pawn pawn && pawn.RaceProps.Humanlike)
                {
                    val *= 1 + ((GetSkinMultByWeight(req) - 1) * GlobalSettings.meatMultiplierForWeight.threshold);
                    return;
                }
            }
            return;
        }

        float GetSkinMultByWeight(StatRequest req)
        {
            if (req.HasThing)
            {
                if (req.Thing is Pawn pawn)
                {
                    if (pawn.WeightHediff() is Hediff weightHediff)
                    {
                        float weight = Utilities.HediffUtility.SeverityToKilosWithBaseWeight(weightHediff.Severity);
                        return (0.0006f * weight + 0.98f);
                    }
                }
            }

            return 1;
        }
    }
}
