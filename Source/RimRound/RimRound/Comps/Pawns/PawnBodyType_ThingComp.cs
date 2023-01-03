using AlienRace;
using RimRound.UI;
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
    public class PawnBodyType_ThingComp : ThingComp
    {
        public PawnBodyType_ThingComp() 
        {
            gizmo = new PersonalDynamicBodyGizmo(this);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<BodyArchetype>(ref _bodyarchetype, "_bodyarchetype", BodyArchetype.none, false);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (GlobalSettings.showExemptionGizmo)
                yield return gizmo;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (((Pawn)parent).RaceProps.Humanlike)
            {
                BodyTypeUtility.AssignPersonalCategoricalExemptions(this);

                if (BodyTypeUtility.GetBodyTypeBasedOnWeightSeverity(((Pawn)parent), personallyExempt, categoricallyExempt) is BodyTypeDef b && b != ((Pawn)parent).story.bodyType)
                {
                    ((Pawn)parent).story.bodyType = b;
                }
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (((Pawn)parent).RaceProps.Humanlike)
            {
                ticksSinceLastBodyChange += 250;
                if (ticksSinceLastBodyChange >= numberOfTicksCooldownPerChange)
                {
                    BodyTypeUtility.UpdatePawnSprite((Pawn)parent, personallyExempt, categoricallyExempt, false, true);
                    ticksSinceLastBodyChange = 0;
                }
            }
        }

        public int ticksSinceLastBodyChange = 0;
        private readonly int numberOfTicksCooldownPerChange = 250;

        bool categoricallyExempt = false;
        public bool CategoricallyExempt
        {
            get => categoricallyExempt;
            set
            {
                categoricallyExempt = value;
            }
        }



        bool personallyExempt = false;
        public bool PersonallyExempt 
        { 
            get => personallyExempt; 
            set 
            {
                personallyExempt = value;
                BodyTypeUtility.UpdatePawnSprite((Pawn)parent, personallyExempt, categoricallyExempt, true, true);
            } 
        }

        readonly PersonalDynamicBodyGizmo gizmo;

        public bool usingCustomBodyMeshSize = false;

        BodyArchetype _bodyarchetype = BodyArchetype.none;
        public BodyArchetype BodyArchetype 
        {
            get 
            {
                if (_bodyarchetype == BodyArchetype.none)
                {
                    if (Values.RandomChanceAtOrBelow(0.5f))
                    {
                        _bodyarchetype = BodyArchetype.standard;
                    }
                    else
                    {
                        _bodyarchetype = BodyArchetype.apple;
                    }
                }

                return _bodyarchetype;
            }
            set 
            {
                _bodyarchetype= value;
            }
        }
    }

    public enum BodyArchetype 
    {
        none,
        standard,
        apple,
    }
}
