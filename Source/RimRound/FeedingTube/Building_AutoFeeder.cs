using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimRound.FeedingTube
{
    public class Building_AutoFeeder : Building
    {
        AutoFeederMode currentMode = AutoFeederMode.off;
        Pawn currentPawn;

        CompEquippable compEquippable;

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            { 
                yield return gizmo;
            }

            if (currentPawn == null)
                yield return GetStartActionGizmo();
            else
                yield return GetStopActionGizmo();

        }

        Command_VerbTarget GetStartActionGizmo() 
        {
            Command_VerbTarget command_VerbTarget = new Command_VerbTarget();
            command_VerbTarget.defaultLabel = "CommandSetForceAttackTarget".Translate();
            command_VerbTarget.defaultDesc = "CommandSetForceAttackTargetDesc".Translate();
            command_VerbTarget.icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", true);
            if (compEquippable is null)
                compEquippable = this.TryGetComp<CompEquippable>();

            if (compEquippable is null)
                Log.Message("Failed to get CompEquipable!");

            command_VerbTarget.verb = compEquippable.PrimaryVerb;
            command_VerbTarget.drawRadius = false;

            //command_VerbTarget.Disable("CannotFire".Translate() + ": " + "Roofed".Translate().CapitalizeFirst());

            return command_VerbTarget;
        }

        Command_Action GetStopActionGizmo() 
        {
            Command_Action command_Action = new Command_Action();
            command_Action.defaultLabel = "CommandStopForceAttack".Translate();
            command_Action.defaultDesc = "CommandStopForceAttackDesc".Translate();
            command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/Halt", true);
            command_Action.action = delegate ()
            {
                currentPawn = null;
                SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
            };

            //command_Action.Disable("CommandStopAttackFailNotForceAttacking".Translate());

            return command_Action;
        }

    }



    public enum AutoFeederMode 
    {
        off,
        lose,
        maintain,
        gain,
        maxgain,
    }
}
