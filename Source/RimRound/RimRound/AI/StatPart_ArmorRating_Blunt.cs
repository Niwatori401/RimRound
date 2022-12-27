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
    public class StatPart_ArmorRating_Blunt : StatPart
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

                    int titanicTankLevel = comp.perkLevels.PerkToLevels?["RR_TitanicTank_Title"] ?? 0;

                    val += titanicTankLevel * 0.1f;

                    return "RR_TitanicTank_Title".Translate() + $" {(val * 100):f1}%";
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

                    int titanicTankLevel = comp.perkLevels.PerkToLevels?["RR_TitanicTank_Title"] ?? 0;

                    val += titanicTankLevel * 0.1f;
                }
            }
        }
    }
}
