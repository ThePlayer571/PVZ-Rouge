using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Tomb;

namespace TPL.PVZR.Models
{
    public interface IGameModel : IModel
    {
        IGameData GameData { get; set; }
        
        /// <summary>
        /// 当前正在游玩的Tomb
        /// </summary>
        ITombData ActiveTombData { get; set; }
    }

    public class GameModel : AbstractModel, IGameModel
    {
        protected override void OnInit()
        {
        }

        public IGameData GameData { get; set; }
        public ITombData ActiveTombData { get; set; }
    }
}