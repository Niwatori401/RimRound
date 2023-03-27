using RimRound.AI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.AI
{
    public class HistoryAutoRecorderGroupWeight : IExposable
    {
        private static HistoryAutoRecorderGroupWeight _instance;
        public static HistoryAutoRecorderGroupWeight Instance() 
        {
            if (HistoryAutoRecorderGroupWeight._instance is null)
                HistoryAutoRecorderGroupWeight._instance = new HistoryAutoRecorderGroupWeight();

            _instance.def = Defs.HistoryAutoRecorderGroupDefOf.RR_PawnWeightHistory;
            return _instance;
        }

        public float GetMaxDay()
        {
            float num = 0f;
            foreach (HistoryAutoRecorderWeight historyAutoRecorder in this.recorders)
            {
                int count = historyAutoRecorder.records.Count;
                if (count != 0)
                {
                    float num2 = (count - 1) * historyAutoRecorder.def.recordTicksFrequency / 60000f;
                    if (num2 > num)
                    {
                        num = num2;
                    }
                }
            }
            return num;
        }

        public void Tick()
        {
            for (int i = 0; i < this.recorders.Count; i++)
            {
                this.recorders[i].Tick();
            }
        }

        public void DrawGraph(Rect graphRect, Rect legendRect, FloatRange section, List<CurveMark> marks)
        {
            int ticksGame = Find.TickManager.TicksGame;
            if (ticksGame != this.cachedGraphTickCount)
            {
                this.cachedGraphTickCount = ticksGame;
                this.curves.Clear();
                List<HistoryAutoRecorderWeight> weightCurvesToShow = new List<HistoryAutoRecorderWeight>();

                for (int i = 0; i < this.recorders.Count; i++)
                {
                    if (Find.Selector.SelectedPawns.Where((pawn) => { return pawn.ThingID == this.recorders[i].pawnToShowInfoFor.ThingID; }).Any())
                        weightCurvesToShow.Add(this.recorders[i]);
                }

                foreach (var recorder in weightCurvesToShow)
                {
                    SimpleCurveDrawInfo simpleCurveDrawInfo = new SimpleCurveDrawInfo
                    {
                        color = recorder.def.graphColor,
                        label = recorder.def.LabelCap,
                        valueFormat = recorder.def.valueFormat,
                        curve = new SimpleCurve()
                    };

                    for (int j = 0; j < recorder.records.Count; j++)
                        simpleCurveDrawInfo.curve.Add(new CurvePoint((float)j * (float)recorder.def.recordTicksFrequency / 60000f, recorder.records[j]), false);

                    simpleCurveDrawInfo.curve.SortPoints();

                    if (recorder.records.Count == 1)
                        simpleCurveDrawInfo.curve.Add(new CurvePoint(1.6666667E-05f, recorder.records[0]), true);

                    this.curves.Add(simpleCurveDrawInfo);
                }
            }

            if (Mathf.Approximately(section.min, section.max))
                section.max += 1.6666667E-05f;

            SimpleCurveDrawerStyle curveDrawerStyle = Find.History.curveDrawerStyle;
            curveDrawerStyle.FixedSection = section;
            curveDrawerStyle.UseFixedScale = this.def.useFixedScale;
            curveDrawerStyle.FixedScale = this.def.fixedScale;
            curveDrawerStyle.YIntegersOnly = this.def.integersOnly;
            curveDrawerStyle.OnlyPositiveValues = this.def.onlyPositiveValues;
            SimpleCurveDrawer.DrawCurves(graphRect, this.curves, curveDrawerStyle, marks, legendRect);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look<HistoryAutoRecorderGroupDef>(ref this.def, "def");
            Scribe_Collections.Look<HistoryAutoRecorderWeight>(ref this.recorders, "recorders", LookMode.Deep, Array.Empty<object>());
        }

        public void AddHistoryRecorders(Pawn pawn)
        {
            if (this.recorders.RemoveAll((HistoryAutoRecorderWeight x) => x == null) != 0)
                Log.Warning("Some history auto recorders were null.");


            if (this.recorders.Any((r) => { return pawn.ThingID == r.pawnToShowInfoFor.ThingID; }))
                return;

            var recorder = new HistoryAutoRecorderWeight
            {
                pawnToShowInfoFor = pawn,
                def = Defs.HistoryAutoRecorderDefOf.RR_Weight_Pawn
            };

            this.recorders.Add(recorder);
        }

        public HistoryAutoRecorderGroupDef def;

        public List<HistoryAutoRecorderWeight> recorders = new List<HistoryAutoRecorderWeight>();

        private List<SimpleCurveDrawInfo> curves = new List<SimpleCurveDrawInfo>();

        private int cachedGraphTickCount = -1;
    }
}
