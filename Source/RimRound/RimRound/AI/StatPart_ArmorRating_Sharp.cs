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
    public class StatPart_ArmorRating_Sharp : StatPart
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

                    int bulletProofBlobLevel = comp.perkLevels.PerkToLevels?["RR_Bulletproof_Blob_Title"] ?? 0;

                    if (!BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_090_Titanic))
                        bulletProofBlobLevel = 0;

                    int thatLevel = comp.perkLevels.PerkToLevels?["RR_That_Title"] ?? 0;

                    if (!BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_500_Gelatinous))
                        thatLevel = 0;


                    return "RR_Bulletproof_Blob_Title".Translate() + $" {bulletProofBlobLevel * 0.1f * 100:f1}%" + "\n" + "RR_That_Title".Translate() + $" {thatLevel * 0.5f * 100:f1}%";
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

                    int bulletProofBlobLevel = comp.perkLevels.PerkToLevels?["RR_Bulletproof_Blob_Title"] ?? 0;

                    if (!BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_090_Titanic))
                        bulletProofBlobLevel = 0;

                    int thatLevel = comp.perkLevels.PerkToLevels?["RR_That_Title"] ?? 0;

                    if (!BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_500_Gelatinous))
                        thatLevel = 0;

                    val += bulletProofBlobLevel * 0.1f + thatLevel * 0.5f;
                }
            }
        }
    }
}
