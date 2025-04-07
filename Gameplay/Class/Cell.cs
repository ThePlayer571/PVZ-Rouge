using TPL.PVZR.Gameplay.Entities.Plants.Base;

namespace TPL.PVZR.Gameplay.Class
{
    public class Cell
    {
        public enum CellState
        {
            Empty, HavePlant, HaveStone,HaveDirt, HaveFlowerpot
        }

        public CellState cellState { set; private get; }
        public Plant plant;
        
        //
        public bool IsEmpty => cellState is CellState.Empty;
        public bool HaveFlowerpot => cellState is CellState.HaveFlowerpot;
        public bool HavePlant => cellState is CellState.HavePlant or CellState.HaveFlowerpot;
        // 种植植物
        public bool CanPlantHere => cellState is CellState.Empty;
        public bool CanPlantAbove => cellState is CellState.HaveDirt or CellState.HaveFlowerpot;
        public bool CanPotAbove => cellState is CellState.HaveDirt or CellState.HaveStone or CellState.HaveFlowerpot;
    }


}
