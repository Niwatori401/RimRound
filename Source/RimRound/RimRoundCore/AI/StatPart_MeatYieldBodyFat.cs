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
    public class StatPart_MeatYieldBodyFat : StatPart
    {
        public override string ExplanationPart(StatRequest req)
        {
            if (req.Thing is Pawn p && p.RaceProps.Humanlike) 
            {
                float mult = GetMeatMultByWeight(req);

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
                    val *= GetMeatMultByWeight(req);
                    return;
                }
            }
            return;
        }

        float GetMeatMultByWeight(StatRequest req) 
        {
            if (req.HasThing)
            {
                if (req.Thing is Pawn pawn)
                {
                    if (pawn.WeightHediff() is Hediff weightHediff)
                    {
                        return ((Utilities.HediffUtility.SeverityToKilosWithBaseWeight(weightHediff.Severity) + 225) * 0.5f) / 140;
                    }
                }
            }

            return 1;
        }
    }
}
