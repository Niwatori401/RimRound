using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Defs
{
    [DefOf]
    public static class PawnCapacityDefOf
    {
        static PawnCapacityDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PawnCapacityDefOf));
        }

        // This was seemingly removed from the vanilla PawnCapacityDefOf for some reason
        public static PawnCapacityDef Eating;
    }
}
