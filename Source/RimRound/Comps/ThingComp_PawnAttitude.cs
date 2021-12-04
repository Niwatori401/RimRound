using RimRound.Enums;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Comps
{
    public class ThingComp_PawnAttitude : ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

        }

        public WeightOpinion weightOpinion;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Increase Weight Opinion",
                    action = delegate ()
                    {
                        IncreaseOpinion();
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "Decrease Weight Opinion",
                    action = delegate ()
                    {
                        LowerOpinion();
                    }
                };

            }
        }

        public WeightOpinion IncreaseOpinion() 
        {
            List<WeightOpinion> weightOpinions = new List<WeightOpinion>();

            foreach (WeightOpinion x in Enum.GetValues(typeof(WeightOpinion)))
            {
                weightOpinions.Add(x);
            }

            int currentIndex = -1;

            for (int i = 0; i < weightOpinions.Count; ++i)
            {
                if (weightOpinions[i] == this.weightOpinion)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex == -1)
                Log.Warning("Index not found in ThingComp_PawnAttitude!");

            if (currentIndex != weightOpinions.Count - 1)
                this.weightOpinion = weightOpinions[currentIndex + 1];

            if (Prefs.DevMode)
                Log.Message($"Current opinion is now: {weightOpinion}");

            if (!((Pawn)parent).story.traits.HasTrait(Functions.GetTraitByWeightOpinion(this.weightOpinion)))
            {
                Functions.RemoveWeightOpinionTraits((Pawn)parent);
                ((Pawn)parent).story.traits.GainTrait(new Trait(Functions.GetTraitByWeightOpinion(this.weightOpinion)));
            }


            return weightOpinion;
        }

        public WeightOpinion LowerOpinion() 
        {
            List<WeightOpinion> weightOpinions = new List<WeightOpinion>();

            foreach (WeightOpinion x in Enum.GetValues(typeof(WeightOpinion)))
            {
                weightOpinions.Add(x);
            }

            int currentIndex = -1;

            for (int i = 0; i < weightOpinions.Count; ++i)
            {
                if (weightOpinions[i] == this.weightOpinion)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex == -1)
                Log.Warning("Index not found in ThingComp_PawnAttitude!");

            if (currentIndex != 0) // weightOpinions.Last() will cause it to wrap around.
                this.weightOpinion = weightOpinions[currentIndex - 1];

            if (Prefs.DevMode)
                Log.Message($"Current opinion is now: {weightOpinion}");

            if (!((Pawn)parent).story.traits.HasTrait(Functions.GetTraitByWeightOpinion(this.weightOpinion)))
            {
                Functions.RemoveWeightOpinionTraits((Pawn)parent);
                ((Pawn)parent).story.traits.GainTrait(new Trait(Functions.GetTraitByWeightOpinion(this.weightOpinion)));
            }

            return weightOpinion;
        }
    }
}
