using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RimRound.AI
{
    public class WorkGiver_EatAtFoodDispenser : WorkGiver_Scanner
    {
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(Defs.ThingDefOf.RR_TD_FoodFaucet);
			}
		}

		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.InteractionCell;
			}
		}

		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t.Faction != pawn.Faction)
			{
				return false;
			}
			Building_FoodFaucet building = t as Building_FoodFaucet;
			return 
				building != null && !building.IsForbidden(pawn) && 
				pawn.CanReserve(building, 1, -1, null, forced) && 
				(building.TryGetComp<FoodNetTrader_ThingComp>()?.CanBeOn ?? false) && 
				building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null && 
				!building.IsBurning();
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(JobDefOf.OperateDeepDrill, t, 1500, true);
		}
	}
}
