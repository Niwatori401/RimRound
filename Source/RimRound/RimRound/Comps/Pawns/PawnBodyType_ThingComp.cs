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
            dynamicBodyExcemptionGizmo = new PersonalDynamicBodyGizmo(this);
            customBodyTypeGizmo = new CustomBodyTypeGizmo(this);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<BodyArchetype>(ref _bodyarchetype, "_bodyarchetype", BodyArchetype.none, false);
            Scribe_Values.Look<string>(ref bodyTypeDictNameString, "bodyTypeDictNameString", null, false);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (GlobalSettings.showExemptionGizmo)
                yield return dynamicBodyExcemptionGizmo;

            if (Prefs.DevMode)
                yield return customBodyTypeGizmo;
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
                }
            }
        }



        public Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> CustomBodyTypeDict 
        {
            get 
            {
                if (bodyTypeDictNameString is null)
                    return null;

                Dictionary<BodyArchetype, Dictionary<BodyTypeDef, BodyTypeInfo>> returnValue = null;
                RacialBodyTypeInfoUtility.genderedSets.TryGetValue(bodyTypeDictNameString, out returnValue);
                return returnValue;
            } 
        }

        public string bodyTypeDictNameString = null;

        public int ticksSinceLastBodyChange = 0;
        public readonly int numberOfTicksCooldownPerChange = 250;

        ExemptionReason categoricallyExempt = false;
        public ExemptionReason CategoricallyExempt
        {
            get => categoricallyExempt;
            set
            {
                categoricallyExempt = value;
            }
        }



        ExemptionReason personallyExempt = false;
        public ExemptionReason PersonallyExempt 
        { 
            get => personallyExempt; 
            set 
            {
                personallyExempt = value;
                BodyTypeUtility.UpdatePawnSprite((Pawn)parent, personallyExempt, categoricallyExempt, true, true);
            } 
        }

        readonly PersonalDynamicBodyGizmo dynamicBodyExcemptionGizmo;

        readonly CustomBodyTypeGizmo customBodyTypeGizmo;

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

    // Converts to true if pawn is exempt. This is inddicated by there being a given reason that they are exempt.
    public struct ExemptionReason
    {
        public ExemptionReason(string reason = null) 
        {
            this.reason = reason;
        }

        public static implicit operator ExemptionReason(bool b) => b ? new ExemptionReason("other reason") : new ExemptionReason();
        public static implicit operator bool(ExemptionReason e) => !(e.reason is null);
        
        public string reason;
    }




    public enum BodyArchetype 
    {
        none,
        standard,
        apple,
    }
}
