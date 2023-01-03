using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace RimRound.Utilities
{
    public class PatchOperationFindModPlusUltra : PatchOperation
    {
        protected override bool ApplyWorker(XmlDocument xml)
        {
            bool isModLoaded = false;
            
            for (int i = 0; i < this.mods.Count; i++)
            {
                string modName = this.mods[i];

                if (!modSuccess.TryGetValue(modName, out isModLoaded)) 
                {
                    if (ModLister.HasActiveModWithName(this.mods[i]))
                    {
                        isModLoaded = true;
                        break;
                    }

                    modSuccess.Add(modName, isModLoaded);
                }
            }

            if (isModLoaded && this.match != null)
            {
                return this.match.Apply(xml);
            }
            else if (this.nomatch != null)
            {
                return this.nomatch.Apply(xml);
            }

            return true;
        }

        public override string ToString()
        {
            return string.Format("{0}({1})", base.ToString(), this.mods.ToCommaList(false, false));
        }

        private List<string> mods;

        private PatchOperation match;

        private PatchOperation nomatch;

        private static Dictionary<string, bool> modSuccess = new Dictionary<string, bool>();
    }
}
