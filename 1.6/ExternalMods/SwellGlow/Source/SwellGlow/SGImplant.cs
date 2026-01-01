using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SwellGlow
{

    public class Recipe_InstallImplantAllowDuplicates : Recipe_InstallImplant
    {
        // vanilla just filters out parts that already have the hediff;
        // we remove that filter so the bill stays available.
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(
            Pawn pawn, RecipeDef recipe)
        {
            return MedicalRecipesUtility.GetFixedPartsToApplyOn(recipe, pawn, delegate (BodyPartRecord record)
            {
                if (!pawn.health.hediffSet.GetNotMissingParts().Contains(record))
                {
                    return false;
                }

                if (pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record))
                {
                    return false;
                }

                return true;
            });
        }
    }
    public class SGImplant : HediffWithComps
    {
        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame % 300 == 0)
            {
                if (pawn != null && pawn.Spawned)
                {
                    FullnessAndDietStats_ThingComp pawnComp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
                    pawnComp.activeWeightGainRequests.Enqueue(new WeightGainRequest(20 * Severity, Find.TickManager.TicksGame + 10, 0, false));
                }
            }
        }
    }
}
