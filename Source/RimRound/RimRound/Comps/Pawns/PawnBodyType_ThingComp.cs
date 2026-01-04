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
        public int bodytypeindex = 0;

        public PawnBodyType_ThingComp() 
        {
            dynamicBodyExcemptionGizmo = new PersonalDynamicBodyGizmo(this);
            customBodyTypeGizmo = new CustomBodyTypeGizmo(this);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<BodyArchetypeNew>(ref _bodyarchetype, "_bodyarchetype", BodyArchetypeNew.none, false);
            Scribe_Values.Look<string>(ref personallyExempt.reason, "personallyExempt", null);
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

                if (BodyTypeUtility.GetBodyTypeBasedOnWeightSeverity(((Pawn)parent), PersonallyExempt, categoricallyExempt) is BodyTypeDef b && b != ((Pawn)parent).story.bodyType)
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
                    BodyTypeUtility.UpdatePawnSprite((Pawn)parent, PersonallyExempt, categoricallyExempt, false, true);
                }
            }
        }



        public Dictionary<BodyArchetypeNew, Dictionary<BodyTypeDef, BodyTypeInfo>> CustomBodyTypeDict 
        {
            get 
            {
                if (bodyTypeDictNameString is null)
                    return null;

                Dictionary<BodyArchetypeNew, Dictionary<BodyTypeDef, BodyTypeInfo>> returnValue = null;
                RacialBodyTypeInfoUtility.genderedSets.TryGetValue(bodyTypeDictNameString, out returnValue);
                return returnValue;
            } 
        }


        public void RegisterListener(IWeightStageChangedListener listener) 
        {
            _weightStageListeners.Add(listener);
        }

        public void RemoveListener(IWeightStageChangedListener listener) 
        {
            bool success = _weightStageListeners.Remove(listener);

            if (!success) 
            {
                Log.Warning($"Tried to remove listener from {this.parent.AsPawn().Name.ToStringFull} but it wasn't registered!");
            }
        }


        public void NotifyWeightStageListeners() 
        {
            foreach (var listener in _weightStageListeners) 
            {
                listener.OnWeightStageChanged();
            }
        }

        private List<IWeightStageChangedListener> _weightStageListeners = new List<IWeightStageChangedListener>();

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



        ExemptionReason personallyExempt = new ExemptionReason();
        public ExemptionReason PersonallyExempt 
        { 
            get => personallyExempt; 
            set 
            {
                personallyExempt = value;
                BodyTypeUtility.UpdatePawnSprite((Pawn)parent, PersonallyExempt, categoricallyExempt, true, true);
            } 
        }

        readonly PersonalDynamicBodyGizmo dynamicBodyExcemptionGizmo;

        readonly CustomBodyTypeGizmo customBodyTypeGizmo;

        public bool usingCustomBodyMeshSize = false;

        BodyArchetypeNew _bodyarchetype = BodyArchetypeNew.none;
        public BodyArchetypeNew BodyArchetype 
        {
            get 
            {
                if (_bodyarchetype == BodyArchetypeNew.none)
                {
                    if (Values.RandomChanceAtOrBelow(0.5f))
                    {
                        _bodyarchetype = BodyArchetypeNew.BambooStandard;
                    }
                    else
                    {
                        _bodyarchetype = BodyArchetypeNew.BambooApple;
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
    public class ExemptionReason
    {
        public ExemptionReason() 
        {
            this.reason = null;
        }

        public ExemptionReason(string reason) 
        {
            this.reason = reason;
        }

        public static implicit operator ExemptionReason(bool b) => b ? new ExemptionReason("other reason") : new ExemptionReason();
        public static implicit operator bool(ExemptionReason e) => !(e.reason is null);
        
        public string reason;
    }




    public enum BodyArchetypeNew 
    {
        none,
        BambooStandard,
        BambooApple,
        GosukeVanillaPlusFemale,
        TogglePear,
        
        ArtOfFire,
        GosukeRedraw,
        GosukeVanillaPlusMale,
        MeatSlop,
    }
}
