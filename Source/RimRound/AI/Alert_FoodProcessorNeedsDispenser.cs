using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.AI
{
    class Alert_FoodProcessorNeedsDispenser : Alert
    {
		public Alert_FoodProcessorNeedsDispenser()
		{
			this.defaultLabel = "NeedFoodHopper".Translate();
			this.defaultExplanation = "NeedFoodHopperDesc".Translate();
			this.defaultPriority = AlertPriority.High;
		}

		private List<Thing> FoodProcessorsWithoutHoppers
		{
			get
			{
				this.badProcessors.Clear();
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
								this.badProcessors.Add(thing);
							}
						}
					}
				}
				return this.badProcessors;
			}
		}

		// Token: 0x060073D4 RID: 29652 RVA: 0x0026E294 File Offset: 0x0026C494


		// Token: 0x060073D5 RID: 29653 RVA: 0x0026E2E3 File Offset: 0x0026C4E3
		public override AlertReport GetReport()
		{
			return AlertReport.CulpritsAre(this.FoodProcessorsWithoutHoppers);
		}

		// Token: 0x04003F91 RID: 16273
		private List<Thing> badProcessors = new List<Thing>();
	}
}

