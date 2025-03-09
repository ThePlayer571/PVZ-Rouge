using UnityEngine;
using QFramework;
using TPL.PVZR.EntityPlant;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


namespace TPL.PVZR
{
    public class Cell
    {
        public enum CellState
        {
            Empty,HavePlant, HaveTile, HaveFlowerpot
        }

        public CellState cellState { set; private get; }
        public Plant plant;
        
        //
        public bool IsEmpty => cellState is CellState.Empty;
        public bool HaveFlowerpot => cellState is CellState.HaveFlowerpot;
        public bool HavePlant => cellState is CellState.HavePlant or CellState.HaveFlowerpot;
        public bool CanPlantOn => cellState is CellState.HaveTile or CellState.HaveFlowerpot;
        public bool CanPotOn => cellState is CellState.HaveTile or CellState.HaveFlowerpot;
    }


}
