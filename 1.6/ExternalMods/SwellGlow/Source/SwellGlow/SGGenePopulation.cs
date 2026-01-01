using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SwellGlow
{
    public static class SwellGlow_Startup
    {
        static SwellGlow_Startup()
        {
            new Harmony("swellglow.genepatches").PatchAll();
        }
    }

    public class SwellGlow_GameComponent : GameComponent
    {
        private bool initialized = false;

        public SwellGlow_GameComponent(Game game) : base()
        {
        }

        public override void FinalizeInit()
        {
            // Ensure we only run once per save
            if (initialized)
                return;
            initialized = true;

            // ----------------------------- //
            // Gather all Weight‑Opinion genes
            // ----------------------------- //
            List<GeneDef> opinionGenes = DefDatabase<GeneDef>.AllDefs
                .Where(g => g.exclusionTags != null &&
                            g.exclusionTags.Contains("WeightOpinionGene"))
                .ToList();

            if (opinionGenes.Count == 0)
            {
                Log.Warning("[SwellGlow] No WeightOpinion genes found; FinalizeInit skipped.");
                return;
            }

            // ----------------------------------------------------------- //
            // Iterate over every pawn currently present in the save
            // ----------------------------------------------------------- //
            foreach (Pawn pawn in PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead)
            {
                if (pawn == null || pawn.Dead || pawn.genes == null)
                    continue;

                // Skip pawns that already have one of our genes
                bool alreadyHasGene = pawn.genes.GenesListForReading.Any(g =>
                    g.def.exclusionTags != null &&
                    g.def.exclusionTags.Contains("WeightOpinionGene"));
                if (alreadyHasGene)
                    continue;

                // --------------------------- //
                // 1. Detect WeightOpinion via traits
                // --------------------------- //
                WeightOpinion? pawnOpinion = null;

                if (pawn.story != null &&
                    pawn.story.traits != null &&
                    pawn.story.traits.allTraits != null)
                {
                    foreach (Trait t in pawn.story.traits.allTraits)
                    {
                        SGTraitExtension ext = t.def.GetModExtension<SGTraitExtension>();
                        if (ext != null)
                        {
                            pawnOpinion = ext.opinionTrait;
                            break;  // first matching trait is enough
                        }
                    }
                }

                // --------------------------- //
                // 2. Choose a gene
                // --------------------------- //
                GeneDef chosenGene = null;

                if (pawnOpinion.HasValue)
                {
                    foreach (GeneDef g in opinionGenes)
                    {
                        SGTraitExtension gExt = g.GetModExtension<SGTraitExtension>();
                        if (gExt != null && gExt.opinionTrait == pawnOpinion.Value)
                        {
                            chosenGene = g;
                            break;
                        }
                    }
                }

                // 3. Fallback to random if no exact match
                if (chosenGene == null)
                {
                    chosenGene = opinionGenes.RandomElement();
                }

                // --------------------------- //
                // 4. Add the gene (as an endogene)
                // --------------------------- //
                pawn.genes.AddGene(chosenGene, xenogene: false);
            }

            Log.Message("[SwellGlow] FinalizeInit: synchronized WeightOpinion genes for all pawns.");
        }
    }
    public static class Patch_PawnGenerator_GenerateGenes
    {
        // Postfix runs *after* the original method.
        public static void Postfix(Pawn pawn)
        {
            if (pawn?.genes == null) return;

            // Bail out if the pawn already got one of our genes
            bool alreadyHas = pawn.genes.GenesListForReading.Any(g =>
                g.def.exclusionTags?.Contains("WeightOpinionGene") == true);
            if (alreadyHas) return;

            // ------------------------------------------------------------------
            // 1. Collect all weight‑opinion genes (share the same exclusion tag)
            // ------------------------------------------------------------------
            var candidates = DefDatabase<GeneDef>.AllDefs
                .Where(d => d.exclusionTags?.Contains("WeightOpinionGene") == true)
                .ToList();
            if (!candidates.Any()) return;   // nothing to add

            // ------------------------------------------------------------------
            // 2. Try to discover the pawn’s WeightOpinion from its traits
            // ------------------------------------------------------------------
            WeightOpinion? pawnOpinion = pawn.story?.traits?.allTraits
                .Select(t => t.def.GetModExtension<SGTraitExtension>())
                .Where(ext => ext != null)
                .Select(ext => (WeightOpinion?)ext.opinionTrait)
                .FirstOrDefault(o => o != null);

            // ------------------------------------------------------------------
            // 3. Find a gene whose extension matches that opinion
            // ------------------------------------------------------------------
            GeneDef pick = null;
            if (pawnOpinion is WeightOpinion opinion)
            {
                pick = candidates.FirstOrDefault(g =>
                    g.GetModExtension<SGTraitExtension>()?.opinionTrait == opinion);
            }

            // 4. If no exact match (or no trait found), just pick at random
            if (pick == null)
            {
                pick = candidates.RandomElement();
            }

            pawn.genes.AddGene(pick, xenogene: false);
        }
    }
}
