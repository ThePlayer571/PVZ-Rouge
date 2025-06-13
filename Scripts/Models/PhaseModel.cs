using System.Collections.Generic;
using QFramework;

namespace Models
{
    interface IPhaseModel : IModel
    {
        GamePhase GamePhase { get; }

        /// <summary>
        /// 切换当前的游戏进程（会自动检查是否合理）
        /// </summary>
        /// <param name="changeToPhase"></param>
        /// <param name="parameters"></param>
        void ChangePhase(GamePhase changeToPhase, Dictionary<string, object> parameters = null);
    }

    public class PhaseModel : AbstractModel, IPhaseModel
    {
        #region Public

        public GamePhase GamePhase { get; private set; } = GamePhase.BeforeStart;

        public void ChangePhase(GamePhase changeToPhase, Dictionary<string, object> parameters = null)
        {
            
        }

        #endregion


        private readonly Dictionary<GamePhase, GamePhase[]> allowedPhaseToFrom = new()
        {
            [GamePhase.PreInitialization] = new[] { GamePhase.BeforeStart },
            [GamePhase.MainMenu] = new[] { GamePhase.PreInitialization },
            [GamePhase.GameInitialization] = new[] { GamePhase.MainMenu },
            [GamePhase.MazeMap] = new[] { GamePhase.GameInitialization, GamePhase.LevelExiting },
            [GamePhase.LevelPreInitialization] = new[] { GamePhase.MazeMap },
            [GamePhase.LevelInitialization] = new[] { GamePhase.LevelPreInitialization },
            [GamePhase.ChooseCards] = new[] { GamePhase.LevelInitialization },
            [GamePhase.Gameplay] = new[] { GamePhase.ChooseCards },
            [GamePhase.AllEnemyKilled] = new[] { GamePhase.Gameplay },
            [GamePhase.ChooseLoots] = new[] { GamePhase.AllEnemyKilled },
            [GamePhase.GameOverDefeat] = new[] { GamePhase.Gameplay },
            [GamePhase.LevelExiting] = new[] { GamePhase.ChooseLoots },
        };


        protected override void OnInit()
        {
            throw new System.NotImplementedException();
        }
    }
}