using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.UI
{
    class Alert_FoodProcessorNeedsDispenser : Alert
    {
		public Alert_FoodProcessorNeedsDispenser()
		{
			this.defaultLabel = "NeedFoodHopper".Translate();
			this.defaultExplanation = "NeedFoodHopperProcessorDesc".Translate();
			this.defaultPriority = AlertPriority.High;
		}

		private List<Thing> FoodProcessorsWithoutHoppers
		{
			get
			{
				List<Thing> badProcessors = new List<Thing>();

				badProcessors.Clear();
				List<Map> maps = Find.Maps;
				for (int i = 0; i < maps.Count; i++)
				{
					foreach (Thing thing in maps[i].listerBuildings.allBuildingsColonist)
					{
						if (thing is Building_FoodProcessor)
						{
							bool hasHopper = false;
							ThingDef hopper = ThingDefOf.Hopper;
							foreach (IntVec3 c in GenAdj.CellsAdjacentCardinal(thing))
							{
								Thing edifice = c.GetEdifice(thing.Map);
								if (edifice != null && edifice.def == hopper)
								{
									hasHopper = true;
									break;
								}
							}
							if (!hasHopper)
							{
								badProcessors.Add(thing);
							}
						}
					}
				}
				return badProcessors;
			}
		}

		public override AlertReport GetReport()
		{
			return AlertReport.CulpritsAre(this.FoodProcessorsWithoutHoppers);
		}
	}
}

