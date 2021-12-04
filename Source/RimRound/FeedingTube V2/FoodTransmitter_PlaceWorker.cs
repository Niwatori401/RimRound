using RimRound.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound
{
    public class FoodTransmitter_PlaceWorker : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			List<Thing> thingList = loc.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (EverTransmitsFood(thingList[i].def))
				{
					return false;
				}
				if (thingList[i].def.entityDefToBuild != null)
				{
                    if (thingList[i].def.entityDefToBuild is ThingDef thingDef && EverTransmitsFood(thingDef))
                    {
                        return false;
                    }
                }
			}
			return true;
		}

		bool EverTransmitsFood(ThingDef t)
		{
			foreach (CompProperties cp in t.comps) 
			{
                if (cp is FoodTransmitter_CompProperties)
                {
                    return true;
                }
            }
			return false;
		}


	}
}
