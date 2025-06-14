using System;
using QFramework;
using TPL.PVZR.Classes.Game;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Models;

namespace TPL.PVZR.Systems
{
    public interface IGameSystem : ISystem
    {
        void StartGame(GameData gameData);
    }

    public class GameSystem : AbstractSystem, IGameSystem
    {
        #region Public

        #region Model

        public MazeMapController MazeMapController { get; private set; }

        #endregion
        
        
        
        public void StartGame(GameData gameData)
        {
            if (_PhaseModel.GamePhase != GamePhase.MainMenu)
                throw new Exception($"在不正确的阶段开始游戏：{_PhaseModel.GamePhase}");
            
        }

        #endregion

        #region Private

        private IPhaseModel _PhaseModel;

        #endregion

        protected override void OnInit()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
        }
    }
}