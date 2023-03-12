using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Comps
{
    class CompIncreaseHunger : ThingComp
    {
        public CompProperties_IncreaseHunger Props 
        {
            get 
            {
                return (CompProperties_IncreaseHunger)this.props;
            }
        }

        public float AgeDays 
        {
            get 
            {
                return ticksAge / 60000f;
            }
        }

        public float CurrentSeverity 
        {
            get 
            {
                return Props.severityPerDayCurve.Evaluate(AgeDays);
            }
        }

        private void UpdateDictionaryCache() 
        {
            cachedFnDComps.Clear();

            foreach (Pawn p in GeneralUtility.GetAllLivingHumanlikesOnMap(this.parent.Map))
            {
                if (!cachedFnDComps.ContainsKey(p))
                {
                    var fndComp = p.TryGetComp<FullnessAndDietStats_ThingComp>();
                    if (fndComp != null)
                        cachedFnDComps.Add(p, fndComp);
                }
                else 
                {
                    if (p.Dead) 
                    {
                        cachedFnDComps.Remove(p);
                    }
                }
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            UpdateDictionaryCache();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref ticksAge, "age");
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!this.parent.Spawned)
            {
                return;
            }
            ticksAge++;
        }

        int ticksAge = 0;

        Dictionary<Pawn, FullnessAndDietStats_ThingComp> cachedFnDComps = new Dictionary<Pawn, FullnessAndDietStats_ThingComp>();
    }
}
