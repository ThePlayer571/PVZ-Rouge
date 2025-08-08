using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Tomb;

namespace TPL.PVZR.Models
{
    public interface IGameModel : IModel
    {
        bool IsGamePaused { get; set; }
        IGameData GameData { get; set; }

        /// <summary>
        /// 当前正在游玩的Tomb
        /// </summary>
        ITombData ActiveTombData { get; set; }

        void Reset();
    }

    public class GameModel : AbstractModel, IGameModel
    {
        public bool IsGamePaused { get; set; } = false;

        protected override void OnInit()
        {
        }

        public IGameData GameData { get; set; }
        public ITombData ActiveTombData { get; set; }

        public void Reset()
        {
            GameData = null;
            ActiveTombData = null;
            IsGamePaused = false;
        }
    }
}