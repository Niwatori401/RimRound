using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimRound.Comps
{
    public class MapComp_RRGasGrid : MapComponent, IExposable
    {
        private int[] gasDensity;

        private int cycleIndexDiffusion;

        private int cycleIndexDissipation;

        [Unsaved(false)]
        private List<IntVec3> cardinalDirections;

        [Unsaved(false)]
        private List<IntVec3> cellsInRandomOrder;

        [Unsaved(false)]
        private bool anyGasEverAdded;

        public const int MaxGasPerCell = 255;
        private const float CellsToDissipatePerTickFactor = 0.015625f;
        private const float CellsToDiffusePerTickFactor = 0.03125f;
        private const float MaxOverflowFloodfillRadius = 40f;
        private const int MinDiffusion = 17;
        private const int AnyGasCheckIntervalTicks = 600;


        private static Dictionary<RRGasType, byte> gasDissipationRate = new Dictionary<RRGasType, byte>()
        {
            { RRGasType.fatteningGas, 4},
        };

        private static Dictionary<RRGasType, Color> gasToColor = new Dictionary<RRGasType, Color>()
        {
            { RRGasType.fatteningGas, new Color(1, 1, 1, byte.MaxValue) },
        };

        private static readonly FloatRange AlphaRange = new FloatRange(0.2f, 0.8f);



        private static RRGasType[] allGasesToConsider = new RRGasType[] { RRGasType.fatteningGas };

        public bool CalculateGasEffects
        {
            get
            {
                return this.anyGasEverAdded;
            }
        }

        public MapComp_RRGasGrid(Map map) : base(map)
        {
            this.map = map;
            this.gasDensity = new int[map.cellIndices.NumGridCells];
            this.cardinalDirections = new List<IntVec3>();
            this.cardinalDirections.AddRange(GenAdj.CardinalDirections);
            this.cycleIndexDiffusion = Rand.Range(0, map.Area / 2);
        }


        public void RecalculateEverHadGas()
        {
            this.anyGasEverAdded = false;
            for (int i = 0; i < this.gasDensity.Length; i++)
            {
                if (this.gasDensity[i] > 0)
                {
                    this.anyGasEverAdded = true;
                    return;
                }
            }
        }


        public override void MapComponentTick()
        {
            base.MapComponentTick();

            if (!this.CalculateGasEffects)
            {
                return;
            }
            int area = this.map.Area;
            int cellsToCheck = Mathf.CeilToInt((float)area * CellsToDissipatePerTickFactor);
            this.cellsInRandomOrder = this.map.cellsInRandomOrder.GetAll();
            for (int i = 0; i < cellsToCheck; i++)
            {
                if (this.cycleIndexDissipation >= area)
                {
                    this.cycleIndexDissipation = 0;
                }
                this.TryDissipateGasses(CellIndicesUtility.CellToIndex(this.cellsInRandomOrder[this.cycleIndexDissipation], this.map.Size.x));
                this.cycleIndexDissipation++;
            }
            cellsToCheck = Mathf.CeilToInt((float)area * CellsToDiffusePerTickFactor);
            for (int j = 0; j < cellsToCheck; j++)
            {
                if (this.cycleIndexDiffusion >= area)
                {
                    this.cycleIndexDiffusion = 0;
                }
                this.TryDiffuseGasses(this.cellsInRandomOrder[this.cycleIndexDiffusion]);
                this.cycleIndexDiffusion++;
            }
            if (this.map.IsHashIntervalTick(AnyGasCheckIntervalTicks))
            {
                this.RecalculateEverHadGas();
            }
        }


        public bool AnyGasAt(IntVec3 cell)
        {
            return this.AnyGasAt(CellIndicesUtility.CellToIndex(cell, this.map.Size.x));
        }


        private bool AnyGasAt(int idx)
        {
            return this.gasDensity[idx] > 0;
        }



        /// <returns>Returns byte.MaxValue if over the amount of gas at tile.</returns>
        private byte TotalGasAt(IntVec3 cell)
        {
            int index = CellIndicesUtility.CellToIndex(cell, this.map.Size.x);
            int amountOfGas = 0;

            foreach (var gas in allGasesToConsider)
                amountOfGas += DensityAt(index, gas);

            if (amountOfGas > byte.MaxValue)
                return byte.MaxValue;
            else
                return (byte)amountOfGas;
        }


        public byte DensityAt(IntVec3 cell, RRGasType gasType)
        {
            return this.DensityAt(CellIndicesUtility.CellToIndex(cell, this.map.Size.x), gasType);
        }


        private byte DensityAt(int index, RRGasType gasType)
        {
            return (byte)(this.gasDensity[index] >> (ushort)gasType & MaxGasPerCell);
        }


        public float DensityPercentAt(IntVec3 cell, RRGasType gasType)
        {
            return (float)this.DensityAt(cell, gasType) / MaxGasPerCell;
        }


        public void AddGas(IntVec3 cell, RRGasType gasType, int amount, bool canOverflow = true)
        {
            if (amount <= 0 || !this.GasCanMoveTo(cell))
            {
                return;
            }
            this.anyGasEverAdded = true;
            int index = CellIndicesUtility.CellToIndex(cell, this.map.Size.x);
            byte currentDensity = this.DensityAt(index, gasType);

            int overflowAmount;
            currentDensity = this.AdjustedDensity((int)currentDensity + amount, out overflowAmount);


            this.SetDirect(index, currentDensity, gasType);
            this.map.mapDrawer.MapMeshDirty(cell, MapMeshFlag.Gas);
            if (canOverflow && overflowAmount > 0)
            {
                this.Overflow(cell, gasType, overflowAmount);
            }
        }


        private byte AdjustedDensity(int newDensity, out int overflow)
        {
            if (newDensity > MaxGasPerCell)
            {
                overflow = newDensity - MaxGasPerCell;
                return byte.MaxValue;
            }
            overflow = 0;
            if (newDensity < 0)
            {
                return 0;
            }
            return (byte)newDensity;
        }


        public Color ColorAt(IntVec3 cell)
        {
            float totalGas = 0;
            Color totalColor = default;
            foreach (var gas in allGasesToConsider)
            {
                float currentGasAmount = this.DensityAt(cell, gas);

                Color currentColor = gasToColor[gas];

                if (totalGas == 0)
                    totalColor = currentColor;
                else
                {
                    float newGasTotal = totalGas + currentGasAmount;
                    totalColor = totalColor * (totalGas / newGasTotal) + currentColor * (currentGasAmount / newGasTotal);
                }

                totalGas += currentGasAmount;
            }

            totalColor.a = MapComp_RRGasGrid.AlphaRange.LerpThroughRange(totalGas / 765f);
            return totalColor;
        }


        public void Notify_ThingSpawned(Thing thing)
        {
            if (thing.Spawned && thing.def.Fillage == FillCategory.Full)
            {
                foreach (IntVec3 intVec in thing.OccupiedRect())
                {
                    if (this.AnyGasAt(intVec))
                    {
                        this.gasDensity[CellIndicesUtility.CellToIndex(intVec, this.map.Size.x)] = 0;
                        this.map.mapDrawer.MapMeshDirty(intVec, MapMeshFlag.Gas);
                    }
                }
            }
        }


        private void SetDirect(int index, byte gasDensity, RRGasType gasType)
        {
            this.gasDensity[index] &=  ~(0xff << (ushort)gasType);
            this.gasDensity[index] |= (gasDensity << (ushort)gasType);
        }


        private void Overflow(IntVec3 cell, RRGasType gasType, int amount)
        {
            if (amount <= 0)
            {
                return;
            }
            int remainingAmount = amount;
            this.map.floodFiller.FloodFill(cell, (IntVec3 c) => this.GasCanMoveTo(c), delegate (IntVec3 c)
            {
                int num = Mathf.Min(remainingAmount, (int)(byte.MaxValue - this.DensityAt(c, gasType)));
                if (num > 0)
                {
                    this.AddGas(c, gasType, num, false);
                    remainingAmount -= num;
                }
                return remainingAmount <= 0;
            }, GenRadial.NumCellsInRadius(MaxOverflowFloodfillRadius), true, null);
        }


        private void TryDissipateGasses(int cellIndex)
        {
            if (!this.AnyGasAt(cellIndex))
                return;

            bool shouldRedrawMap = false;

            foreach (var gas in allGasesToConsider)
                shouldRedrawMap |= DissipateSingleGas(cellIndex, gas);

            if (shouldRedrawMap)
                this.map.mapDrawer.MapMeshDirty(CellIndicesUtility.IndexToCell(cellIndex, this.map.Size.x), MapMeshFlag.Gas);
        }

        private bool DissipateSingleGas(int cellIndex, RRGasType gasType)
        {
            bool redrawMap = false;
            byte gasAmount = this.DensityAt(cellIndex, gasType);
            if (gasAmount > 0)
            {
                gasAmount = (byte)Math.Max(gasAmount - gasDissipationRate[gasType], 0);
                if (gasAmount == 0)
                {
                    redrawMap = true;
                }
            }

            this.SetDirect(cellIndex, gasAmount, gasType);
            return redrawMap;
        }

        private void TryDiffuseGasses(IntVec3 cell)
        {
            foreach (var gas in allGasesToConsider)
                DiffuseSingleGasAtCell(cell, gas);
        }

        private void DiffuseSingleGasAtCell(IntVec3 cell, RRGasType gasType)
        {
            int currentCellIndex = CellIndicesUtility.CellToIndex(cell, this.map.Size.x);

            int totalGas = TotalGasAt(cell);

            if (totalGas < MinDiffusion)
                return;

            byte densityOfGasAtOriginCell = this.DensityAt(currentCellIndex, gasType);

            this.cardinalDirections.Shuffle<IntVec3>();
            for (int i = 0; i < this.cardinalDirections.Count; i++)
            {
                IntVec3 adjacentCell = cell + this.cardinalDirections[i];

                if (!this.GasCanMoveTo(adjacentCell))
                    continue;

                int indexOfNeighborCell = CellIndicesUtility.CellToIndex(adjacentCell, this.map.Size.x);
                byte densityOfGasAtNeighboringCell = this.DensityAt(indexOfNeighborCell, gasType);

                if (!this.TryDiffuseIndividualGas(ref densityOfGasAtOriginCell, ref densityOfGasAtNeighboringCell))
                    continue;

                this.SetDirect(indexOfNeighborCell, densityOfGasAtNeighboringCell, gasType);
                this.SetDirect(currentCellIndex, densityOfGasAtOriginCell, gasType);

                this.map.mapDrawer.MapMeshDirty(adjacentCell, MapMeshFlag.Gas);

                totalGas = TotalGasAt(cell);
                if (totalGas < MinDiffusion)
                    break;
            }
        }

        private bool TryDiffuseIndividualGas(ref byte gasA, ref byte gasB)
        {
            if (gasA < MinDiffusion)
            {
                return false;
            }
            byte gasDifferential = (byte)(Mathf.Abs(gasA - gasB) / 2);
            if (gasA > gasB && gasDifferential >= MinDiffusion)
            {
                gasA -= gasDifferential;
                gasB += gasDifferential;
                return true;
            }
            return false;
        }


        public bool GasCanMoveTo(IntVec3 cell)
        {
            if (!cell.InBounds(this.map))
            {
                return false;
            }
            if (cell.Filled(this.map))
            {
                Building_Door door = cell.GetDoor(this.map);
                return door != null && door.Open;
            }
            return true;
        }


        private void GetAdjacentCellsAndCount(Building b, bool twoWay, ref IntVec3[] buildingEqualizeCells, ref int beqCellCount) 
        {
            for (int i = 0; i < buildingEqualizeCells.Length; i++)
                buildingEqualizeCells[i] = IntVec3.Invalid;


            if (twoWay)
            {
                IntVec3 cellFront = (b.Position + b.Rotation.FacingCell);

                if (cellFront.IsValid && this.GasCanMoveTo(cellFront))
                {
                    buildingEqualizeCells[beqCellCount] = cellFront;
                    beqCellCount += 1;
                }


                IntVec3 cellBack = (b.Position - b.Rotation.FacingCell);
                if (cellBack.IsValid && this.GasCanMoveTo(cellBack))
                {
                    buildingEqualizeCells[beqCellCount] = cellBack;
                    beqCellCount += 1;
                }
            }
            else
            {
                for (int k = 0; k < 4; k++)
                {
                    IntVec3 adjacentCell = b.Position + GenAdj.CardinalDirections[k];
                    if (adjacentCell.IsValid && this.GasCanMoveTo(adjacentCell))
                    {
                        buildingEqualizeCells[beqCellCount] = adjacentCell;
                        beqCellCount += 1;
                    }
                }
            }

        }

        public void EqualizeGasThroughBuilding(Building b, bool twoWay)
        {
            if (!this.CalculateGasEffects)
                return;

            IntVec3[] buildingEqualizeCells = new IntVec3[4];

            int beqCellCount = 0;

            GetAdjacentCellsAndCount(b, twoWay, ref buildingEqualizeCells, ref beqCellCount);

            if (beqCellCount <= 1)
                return;


            List<Pair<RRGasType, float>> gasAndTotalAmount = new List<Pair<RRGasType, float>>();

            foreach (var gas in allGasesToConsider)
            {
                float totalGasAmount = 0;
                for (int i = 0; i < beqCellCount; ++i)
                    totalGasAmount += this.DensityAt(this.map.cellIndices.CellToIndex(buildingEqualizeCells[i]), gas);


                gasAndTotalAmount.Add(new Pair<RRGasType, float>(gas, totalGasAmount));
            }


            foreach (var gasAmountPair in gasAndTotalAmount)
            {
                byte gasToDistribute = (byte)Mathf.Min(gasAmountPair.Second / beqCellCount, 255);

                for (int l = 0; l < beqCellCount; l++)
                {
                    if (!buildingEqualizeCells[l].IsValid)
                        continue;
                    
                    int cellIndex = this.map.cellIndices.CellToIndex(buildingEqualizeCells[l]);
                    this.SetDirect(cellIndex, gasToDistribute, gasAmountPair.First);
                    this.map.mapDrawer.MapMeshDirty(buildingEqualizeCells[l], MapMeshFlag.Gas);
                }
            }
        }

        public void Debug_ClearAll()
        {
            for (int i = 0; i < this.gasDensity.Length; i++)
            {
                this.gasDensity[i] = 0;
            }
            this.anyGasEverAdded = false;
            this.map.mapDrawer.WholeMapChanged(MapMeshFlag.Gas);
        }

        public void Debug_FillAll()
        {
            for (int i = 0; i < this.gasDensity.Length; i++)
            {
                if (this.GasCanMoveTo(this.map.cellIndices.IndexToCell(i)))
                {
                    this.SetDirect(i, byte.MaxValue, RRGasType.fatteningGas);
                }
            }
            this.anyGasEverAdded = true;
            this.map.mapDrawer.WholeMapChanged(MapMeshFlag.Gas);
        }

        public override void ExposeData()
        {
            MapExposeUtility.ExposeInt(this.map, (IntVec3 c) => (int)this.gasDensity[this.map.cellIndices.CellToIndex(c)], delegate (IntVec3 c, int val)
            {
                this.gasDensity[this.map.cellIndices.CellToIndex(c)] = val;
            }, "RR_GasDensity");
            Scribe_Values.Look<int>(ref this.cycleIndexDiffusion, "RR_cycleIndexDiffusion", 0, false);
            Scribe_Values.Look<int>(ref this.cycleIndexDissipation, "RR_cycleIndexDissipation", 0, false);
        }

        [DebugAction("General", "Add RR gas...", false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap, displayPriority = 100)]
        public static void AddGas()
        {
            IntVec3 cell = Verse.UI.MouseCell();
            float radius = 25f;
            int num = GenRadial.NumCellsInRadius(radius);
            Find.CurrentMap.GetComponent<MapComp_RRGasGrid>().AddGas(cell, RRGasType.fatteningGas, 255 * num, true);
        }

    }
}
