using TPL.PVZR.Gameplay.Entities;
using TPL.PVZR.Gameplay.Entities.Plants;
using TPL.PVZR.Gameplay.Entities.Plants.Base;

namespace TPL.PVZR.Gameplay.Class
{
    public class Cell
    {
        public enum CellState
        {
            Empty, HavePlant, HaveStone,HaveDirt, HaveFlowerpot
        }

        private CellState _cellState;
        private Plant _plant;

        #region 构造函数

        public Cell(CellState cellState)
        {
            this._cellState = cellState;
        }

        #endregion

        #region 公有函数

        public void SetPlant(Plant plant)
        {
            this._plant = plant;
        }

        public void SetState(CellState cellState)
        {
            this._cellState = cellState;
        }
        

        #endregion
        
        #region 公有属性
        public CellState cellState => _cellState;
        public Plant plant => _plant;
        // 一层具象
        public bool IsEmpty => _cellState is CellState.Empty;
        public bool IsStableTile => _cellState is CellState.HaveStone or CellState.HaveDirt;
        public bool HaveFlowerpot => _cellState is CellState.HaveFlowerpot;
        public bool HavePlant => _cellState is CellState.HavePlant or CellState.HaveFlowerpot;
        // 二层具象
        public bool IsPlaceablePlat => _cellState is CellState.HaveDirt or CellState.HaveFlowerpot;

        public bool CanPlantHere(PlantIdentifier plantIdentifier = PlantIdentifier.PeaShooter)
        {
            return _cellState is CellState.Empty;
        }
        public bool CanPlantAbove(PlantIdentifier plantIdentifier)
        {
            if (plantIdentifier is PlantIdentifier.Flowerpot)
            {
                return _cellState is CellState.HaveDirt or CellState.HaveStone or CellState.HaveFlowerpot;
            }
            else
            {
                return _cellState is CellState.HaveDirt or CellState.HaveFlowerpot;
            }
        }
        #endregion
    }


}
