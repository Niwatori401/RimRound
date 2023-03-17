using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRoundExtraWeapons.DamageWorker
{
    public class ME_DamageWorker_Enbiggen : DefModExtension
    {
        public float kilosPerDamage;
        public int ticksBetweenApplication;
        public float kilosPerApplication;
        public float chanceToInflictIllness;
        public int effectDuration;
        public bool triggerMessages;
    }
}
