using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimRound.Utilities
{
    public static class BlobBedUtility
    {
        public static bool IsBlobBed(this Building_Bed bed) 
        {
            return bed.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_z.defName ||
            bed.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_I.defName ||
            bed.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_II.defName ||
            bed.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_III.defName ||
            bed.def.defName == ThingDefOf.SleepingSpot.defName ||
            bed.def.defName == Defs.ThingDefOf.DoubleSleepingSpot.defName;
        }
    }
}
