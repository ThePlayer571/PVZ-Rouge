using QFramework;
using TPL.PVZR.Classes.DataClasses.Game.Interfaces;
using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Models
{
    public interface IGameModel : IModel
    {
        IGameData GameData { get; set; }
    }

    public class GameModel : AbstractModel, IGameModel
    {
        protected override void OnInit()
        {
        }

        public IGameData GameData { get; set; }
    }
}