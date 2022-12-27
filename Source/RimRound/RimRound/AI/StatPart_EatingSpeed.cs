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
    internal class StatPart_EatingSpeed : StatPart
    {
        public override string ExplanationPart(StatRequest req)
        {
            float val = 0;
            if (req.Thing is Pawn pawn)
            {
                if (pawn.RaceProps.Humanlike)
                {
                    FullnessAndDietStats_ThingComp comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
                    if (comp is null)
                        return "";

                    int demonicDevourmentLevel = comp.perkLevels.PerkToLevels?["RR_Demonic_Devourment_Title"] ?? 0;
                    int breakneckbuffetLevel = comp.perkLevels.PerkToLevels?["RR_Breakneck_Buffet_Title"] ?? 0;

                    val += demonicDevourmentLevel * 0.1f + breakneckbuffetLevel * 0.25f;
                }
            }

            return $"Demonic devourment / Breakneck buffet +{val * 100:f1}%";
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

                    int demonicDevourmentLevel = comp.perkLevels.PerkToLevels?["RR_Demonic_Devourment_Title"] ?? 0;
                    int breakneckbuffetLevel = comp.perkLevels.PerkToLevels?["RR_Breakneck_Buffet_Title"] ?? 0;

                    val += demonicDevourmentLevel * 0.1f + breakneckbuffetLevel * 0.25f;
                }
            }
        }
    }
}
