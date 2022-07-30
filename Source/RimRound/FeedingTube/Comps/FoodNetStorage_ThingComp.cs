using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube.Comps
{
    //Battery bar drawing happens here?
    public class FoodNetStorage_ThingComp : FoodTransmitter_ThingComp
    {
        public float Stored { get => this.storedFood; set => this.storedFood = value; }

        public float Capacity { get => this.Props.capacity; }

        public float Remaining 
        { 
            get 
            { 
                return Capacity - Stored; 
            } 
        }

        public new FoodNetStorage_CompProperties Props => (FoodNetStorage_CompProperties)this.props;

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);
            if (signal == "breakdown") 
            {
                //spawn fire?
                this.Stored = 0;
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(base.CompInspectStringExtra());

            sb.Append("RR_FT_RemainingFoodStored".Translate() + ": ");
            sb.Append(Stored.ToString("F1") + "L" + " / " + Capacity.ToString("F1") + "L");

            return sb.ToString().Trim();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            yield return new Command_Action
            {
                defaultLabel = "Purge Contents",
                action = delegate ()
                {
                    this.Stored = 0;
                    
                },
                defaultDesc = "If the mixture becomes too tainted, you can purge the contents of this container to start over.",
                icon = Widgets.GetIconFor(RimWorld.ThingDefOf.Filth_Water),
                order = 401
            };

            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Fill",
                    action = delegate ()
                    {
                        this.Stored = this.Capacity;
                    }
                };


                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Empty",
                    action = delegate ()
                    {
                        this.Stored = 0;
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Change Bar Size x+",
                    action = delegate ()
                    {
                        if (this.parent is Building_FoodVatLarge b)
                        {
                            b.BarSize.x += 0.01f;
                            Log.Message(b.BarSize.ToString());
                        }
                        if (this.parent is Building_FoodVatSmall s)
                        {
                            s.BarSize.x += 0.01f;
                            Log.Message(s.BarSize.ToString());
                        }
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Change Bar Size x-",
                    action = delegate ()
                    {
                        if (this.parent is Building_FoodVatLarge b)
                        {
                            b.BarSize.x -= 0.01f;
                            Log.Message(b.BarSize.ToString());
                        }
                        if (this.parent is Building_FoodVatSmall s)
                        {
                            s.BarSize.x -= 0.01f;
                            Log.Message(s.BarSize.ToString());
                        }
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Change Bar Size y+",
                    action = delegate ()
                    {
                        if (this.parent is Building_FoodVatLarge b)
                        {
                            b.BarSize.y += 0.01f;
                            Log.Message(b.BarSize.ToString());
                        }
                        if (this.parent is Building_FoodVatSmall s)
                        {
                            s.BarSize.y += 0.01f;
                            Log.Message(s.BarSize.ToString());
                        }
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Change Bar Size y-",
                    action = delegate ()
                    {
                        if (this.parent is Building_FoodVatLarge b)
                        {
                            b.BarSize.y -= 0.01f;
                            Log.Message(b.BarSize.ToString());
                        }
                        if (this.parent is Building_FoodVatSmall s)
                        {
                            s.BarSize.y -= 0.01f;
                            Log.Message(s.BarSize.ToString());
                        }
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Change Bar YPOS (+)",
                    action = delegate ()
                    {
                        if (this.parent is Building_FoodVatLarge b)
                        {
                            b.BarYOFF += 0.01f;
                            Log.Message(b.BarYOFF.ToString());
                        }
                        if (this.parent is Building_FoodVatSmall s) 
                        {
                            s.BarYOFF += 0.01f;
                            Log.Message(s.BarYOFF.ToString());
                        }
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Change Bar YPOS (-)",
                    action = delegate ()
                    {
                        if (this.parent is Building_FoodVatLarge b)
                        {
                            b.BarYOFF -= 0.01f;
                            Log.Message(b.BarYOFF.ToString());
                        }
                        if (this.parent is Building_FoodVatSmall s)
                        {
                            s.BarYOFF -= 0.01f;
                            Log.Message(s.BarYOFF.ToString());
                        }
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Change Bar XPOS (+)",
                    action = delegate ()
                    {
                        if (this.parent is Building_FoodVatLarge b)
                        {
                            b.BarXOFF += 0.01f;
                            Log.Message(b.BarXOFF.ToString());
                        }
                        if (this.parent is Building_FoodVatSmall s)
                        {
                            s.BarXOFF += 0.01f;
                            Log.Message(s.BarXOFF.ToString());
                        }
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Change Bar XPOS (-)",
                    action = delegate ()
                    {
                        if (this.parent is Building_FoodVatLarge b)
                        {
                            b.BarXOFF -= 0.01f;
                            Log.Message(b.BarXOFF.ToString());
                        }
                        if (this.parent is Building_FoodVatSmall s)
                        {
                            s.BarXOFF -= 0.01f;
                            Log.Message(s.BarXOFF.ToString());
                        }
                    }
                };
            }
            yield break;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref storedFood, "FoodStored", 0f);
        }

        //Returns amount successfully drawn
        public float Subtract(float amount) 
        {
            if (amount < 0)
            {
                Log.Warning("Tried to use Subtract() to subtract a negative amount of food. Use Add() instead.");
                Add(amount);
            }

            if (amount >= Stored) 
            {
                float amtStored = Stored;
                Stored = 0;
                return amtStored;
            }

            Stored -= amount;
            return amount;
        }

        //Returns amount successfully added
        public float Add(float amount) 
        {
            if (amount < 0)
            {
                Log.Warning("Tried to use Add() to add a negative amount of food. Use Subtract() instead.");
                Subtract(amount);
            }

            if (amount >= Remaining) 
            {
                float amtRemaining = Remaining;
                Stored = Capacity;
                return amtRemaining;
            }

            Stored += amount;
            return amount;
        }


        float storedFood;
    }
}
