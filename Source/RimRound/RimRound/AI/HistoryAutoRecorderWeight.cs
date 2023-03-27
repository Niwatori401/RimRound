using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.AI
{
    public class HistoryAutoRecorderWeight : IExposable
    {
        public void Tick()
        {
            if (Find.TickManager.TicksGame % this.def.recordTicksFrequency == 0 || !this.records.Any())
            {
                var worker = this.def.Worker as HistoryAutoRecorderWorker_PawnWeight;
                worker.currentPawn = pawnToShowInfoFor;
                float item = worker.PullRecord();
                this.records.Add(item);
            }
        }

        public void ExposeData()
        {
            Scribe_Defs.Look<HistoryAutoRecorderDef>(ref this.def, "def");
            Scribe_References.Look<Pawn>(ref pawnToShowInfoFor, "pawnToShowInfoFor");
            byte[] recordsFromBytes = null;
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                recordsFromBytes = this.RecordsToBytes();
            }
            DataExposeUtility.ByteArray(ref recordsFromBytes, "records");
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                this.SetRecordsFromBytes(recordsFromBytes);
            }
        }

        private byte[] RecordsToBytes()
        {
            byte[] array = new byte[this.records.Count * 4];
            for (int i = 0; i < this.records.Count; i++)
            {
                byte[] bytes = BitConverter.GetBytes(this.records[i]);
                for (int j = 0; j < 4; j++)
                {
                    array[i * 4 + j] = bytes[j];
                }
            }
            return array;
        }

        private void SetRecordsFromBytes(byte[] bytes)
        {
            int num = bytes.Length / 4;
            this.records.Clear();
            for (int i = 0; i < num; i++)
            {
                float item = BitConverter.ToSingle(bytes, i * 4);
                this.records.Add(item);
            }
        }

        public HistoryAutoRecorderDef def;

        public List<float> records = new List<float>();

        public Pawn pawnToShowInfoFor;
    }
}
