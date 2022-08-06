using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.UI
{
    internal class ITab_CharacterDesc : ITab
    {
		public ITab_CharacterDesc() 
		{
			this.labelKey = "RR_CharDescLabel";

		}

        protected override void UpdateSize()
        {
            base.UpdateSize();
			this.size = basePawnCardSize;
		}

        private Pawn PawnToShowInfoAbout
		{
			get
			{
				Pawn pawn = null;
				if (base.SelPawn != null)
				{
					pawn = base.SelPawn;
				}
				else
				{
					Corpse corpse = base.SelThing as Corpse;
					if (corpse != null)
					{
						pawn = corpse.InnerPawn;
					}
				}
				if (pawn == null)
				{
					Log.Error("CharacterDesc tab found no selected pawn to display.");
					return null;
				}
				return pawn;
			}
		}

		protected override void FillTab()
        {
			Rect rect = new Rect(17, 17, basePawnCardSize.x, basePawnCardSize.y);
			DrawCharacterDescription(rect);
        }

		void DrawCharacterDescription(Rect rect) 
		{
			if (PawnToShowInfoAbout is Pawn pawn)
			{
				string text = GetTextDescriptionFor(pawn);
				Rect textrect = new Rect(rect.x, rect.y, rect.width, rect.height);
				Widgets.LabelScrollable(textrect, text, ref scrollbarPos);
			}
		}

		string GetTextDescriptionFor(Pawn pawn)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(GetWeightDescriptionFor(pawn));
			sb.Append("\n\n"); 
			sb.Append(GetRaceTidbits(pawn));

			return sb.ToString();
		}

        private string GetWeightDescriptionFor(Pawn pawn)
        {

			BodyTypeDef pawnBodyTypeDef = pawn.story.bodyType;

			string bodytypeCleaned = RacialBodyTypeInfoUtility.GetEquivalentBodyTypeDef(pawnBodyTypeDef).ToString();
			bodytypeCleaned = BodyTypeUtility.ConvertBodyTypeDefDefnameAccordingToSettings(bodytypeCleaned);

			BodyTypeDef cleanedBodyTypeDef = DefDatabase<BodyTypeDef>.GetNamed(bodytypeCleaned, false);

			float weightReqMultiplier = RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(pawn);
			float weightSeverity = pawn.WeightHediff().Severity;
			if (pawn.gender == Gender.Female || pawn.gender == Gender.None)
			{
				if (weightSeverity >= femaleBodyTypeDescriptions.Last().Key * weightReqMultiplier)
					return femaleBodyTypeDescriptions.Last().Value.Translate(pawn.Name.ToStringShort);

				foreach (KeyValuePair<float, string> keyValuePair in femaleBodyTypeDescriptions)
				{
					if (weightSeverity < keyValuePair.Key * weightReqMultiplier)
						return keyValuePair.Value.Translate(pawn.Name.ToStringShort);
				}
			}
			else 
			{
				if (weightSeverity >= maleBodyTypeDescriptions.Last().Key * weightReqMultiplier)
					return maleBodyTypeDescriptions.Last().Value.Translate(pawn.Name.ToStringShort);

				foreach (KeyValuePair<float, string> keyValuePair in maleBodyTypeDescriptions)
				{
					if (weightSeverity < keyValuePair.Key * weightReqMultiplier)
						return keyValuePair.Value.Translate(pawn.Name.ToStringShort);
				}
			}

			return "ERR: Could not find body type desc.";
		}

		private string GetRaceTidbits(Pawn pawn) 
		{
			if (pawn.def.defName.Contains("Ratkin"))
			{
				float weightReqMultiplier = RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(pawn);
				float weightSeverity = pawn.WeightHediff().Severity;

				if (weightSeverity >= ratkinExtraDescriptions.Last().Key * weightReqMultiplier)
					return ratkinExtraDescriptions.Last().Value.Translate(pawn.Name.ToStringShort);

				foreach (KeyValuePair<float, string> keyValuePair in ratkinExtraDescriptions)
				{
					if (weightSeverity < keyValuePair.Key * weightReqMultiplier)
						return keyValuePair.Value.Translate(pawn.Name.ToStringShort);
				}
				
			}
			return "";
		}
		//Max severity value, description key
		static Dictionary<float, string> femaleBodyTypeDescriptions = new Dictionary<float, string>() 
		{							   
				{  0.005f, "RR_Desc_F_Emaciated"          },
				{  0.015f, "RR_Desc_F_Thin"          },
				{  0.035f, "RR_Desc_F_Female"        },
				{  0.050f, "RR_Desc_F_Thick"         },
				{  0.065f, "RR_Desc_F_Chonky"        },
				{  0.090f, "RR_Desc_F_Chubby"        },
				{  0.120f, "RR_Desc_F_Corpulent"     },
				{  0.155f, "RR_Desc_F_Fat"           },
				{  0.200f, "RR_Desc_F_Obese"         },
				{  0.280f, "RR_Desc_F_MorbidlyObese" },
				{  0.350f, "RR_Desc_F_Lardy"         },
				{  0.430f, "RR_Desc_F_Lardy2"        },
				{  0.660f, "RR_Desc_F_Enormous"      },
				{  0.965f, "RR_Desc_F_Gigantic"      },
				{  1.410f, "RR_Desc_F_Titanic"       },
				{  1.860f, "RR_Desc_F_G1"            },																   	
				{  2.460f, "RR_Desc_F_G2"            },
				{  2.960f, "RR_Desc_F_G2"            },
				{  3.960f, "RR_Desc_F_G2"            },
				{  4.960f, "RR_Desc_F_G5"            },
				{  6.460f, "RR_Desc_F_G5"            },
				{  7.960f, "RR_Desc_F_G5"            },
				{  9.960f, "RR_Desc_F_G5"            },
				{  14.46f, "RR_Desc_F_G9"            },
				{  100f  , "RR_Desc_F_G9"           },
		};

		static Dictionary<float, string> maleBodyTypeDescriptions = new Dictionary<float, string>()
		{
				{  0.015f, "RR_Desc_M_Thin"          },
				{  0.035f, "RR_Desc_M_Male"          },
				{  0.050f, "RR_Desc_M_Thick"         },
				{  0.065f, "RR_Desc_M_Chonky"        },
				{  0.090f, "RR_Desc_M_Chubby"        },
				{  0.120f, "RR_Desc_M_Corpulent"     },
				{  0.155f, "RR_Desc_M_Fat"           },
				{  0.200f, "RR_Desc_M_Obese"         },
				{  0.280f, "RR_Desc_M_MorbidlyObese" },
				{  0.430f, "RR_Desc_M_Lardy"         },
				{  0.660f, "RR_Desc_M_Enormous"      },
				{  0.965f, "RR_Desc_M_Gigantic"      },
				{  1.410f, "RR_Desc_M_Titanic"       },
				{  1.860f, "RR_Desc_M_G1"            },
				{  2.460f, "RR_Desc_M_G2"            },
				{  2.960f, "RR_Desc_M_G3"            },
				{  3.960f, "RR_Desc_M_G4"            },
				{  4.960f, "RR_Desc_M_G5"            },
				{  6.460f, "RR_Desc_M_G6"            },
				{  7.960f, "RR_Desc_M_G7"            },
				{  9.960f, "RR_Desc_M_G8"            },
				{  14.46f, "RR_Desc_M_G9"            },
				{  100f  , "RR_Desc_M_G10"           },
		};

		static Dictionary<float, string> ratkinExtraDescriptions = new Dictionary<float, string>()
		{
				{  0.005f, "RR_Desc_F_Ratkin_Emaciated"     },
				{  0.015f, "RR_Desc_F_Ratkin_Thin"          },
				{  0.035f, "RR_Desc_F_Ratkin_Female"        },
				{  0.050f, "RR_Desc_F_Ratkin_Thick"         },
				{  0.065f, "RR_Desc_F_Ratkin_Chonky"        },
				{  0.090f, "RR_Desc_F_Ratkin_Chubby"        },
				{  0.120f, "RR_Desc_F_Ratkin_Corpulent"     },
				{  0.155f, "RR_Desc_F_Ratkin_Fat"           },
				{  0.200f, "RR_Desc_F_Ratkin_Obese"         },
				{  0.280f, "RR_Desc_F_Ratkin_MorbidlyObese" },
				{  0.350f, "RR_Desc_F_Ratkin_Lardy"         },
				{  0.430f, "RR_Desc_F_Ratkin_Lardy2"        },
				{  0.660f, "RR_Desc_F_Ratkin_Enormous"      },
				{  0.965f, "RR_Desc_F_Ratkin_Gigantic"      },
				{  1.410f, "RR_Desc_F_Ratkin_Titanic"       },
				{  1.860f, "RR_Desc_F_Ratkin_G1"            },
				{  2.460f, "RR_Desc_F_Ratkin_G2"            },
				{  2.960f, "RR_Desc_F_Ratkin_G2"            },
				{  3.960f, "RR_Desc_F_Ratkin_G2"            },
				{  4.960f, "RR_Desc_F_Ratkin_G5"            },
				{  6.460f, "RR_Desc_F_Ratkin_G5"            },
				{  7.960f, "RR_Desc_F_Ratkin_G5"            },
				{  9.960f, "RR_Desc_F_Ratkin_G5"            },
				{  14.46f, "RR_Desc_F_Ratkin_G9"            },
				{  100f  , "RR_Desc_F_Ratkin_G9"            },
		};

		private Vector2 scrollbarPos = new Vector2();

		public static Vector2 basePawnCardSize = new Vector2(480f, 455f);

	}
}
