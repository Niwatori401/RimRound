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
    public class StatPart_ComfyTemperatureMin : StatPart
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

                    int bulletProofBlobLevel = comp.perkLevels.PerkToLevels?["RR_WellInsulated_Title"] ?? 0;

                    val -= bulletProofBlobLevel * 5f;

                    return "RR_WellInsulated_Title".Translate() + $" {val:f1}C";
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

                    int wellInsulatedLevel = comp.perkLevels.PerkToLevels?["RR_WellInsulated_Title"] ?? 0;

                    val -= wellInsulatedLevel * 5f;
                }
            }
        }
    }
}
