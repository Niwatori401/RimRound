using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Comps
{
    class CompProperties_IncreaseHunger : CompProperties
    {
		public CompProperties_IncreaseHunger()
		{
			this.compClass = typeof(CompIncreaseHunger);
		}

		public SimpleCurve severityPerDayCurve;
	}
}

