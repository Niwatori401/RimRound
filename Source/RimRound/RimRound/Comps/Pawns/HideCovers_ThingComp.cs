using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimRound.UI;
using RimRound.Utilities;

namespace RimRound.Comps
{
    //This comp works with RimRound.Patches.PawnRenderer_GetBodyPos_HideBlankets
    public class HideCovers_ThingComp : ThingComp
    {
        public HideCovers_ThingComp() : base()
        {
            hideCoversGizmo = new HideCoversGizmo(this);
        }


        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!this.parent.AsPawn().IsColonist && !this.parent.AsPawn().IsPrisonerOfColony && !Prefs.DevMode)
                yield break;

            if (GlobalSettings.showBlanketManagementGizmo)
                yield return hideCoversGizmo;
        }


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.hideCovers, "coversHidden", false, false);
        }


        public bool HideCovers
        {
            get
            {
                return hideCovers;
            }
            set
            {
                BodyTypeUtility.RedrawPawn(parent.AsPawn());
                hideCovers = value;
            }
        }


        private bool hideCovers = false;

        Gizmo hideCoversGizmo;
    }
}
