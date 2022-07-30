using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Things
{
    public class ME_FatteningBullet : DefModExtension
    {
        public float kilosToAdd;
        public int   ticksBetweenApplication;
        public int   numberOfApplications;
        public float chanceToInflictIllness;
        public int   effectDuration;
        public bool  triggerMessages;
    }
}
