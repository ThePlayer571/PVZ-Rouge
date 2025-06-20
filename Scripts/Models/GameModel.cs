using QFramework;
using TPL.PVZR.Classes.Game;
using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Models
{
    public interface IGameModel : IModel
    {
        IGameData GameData { get; set; }
        IMazeMapController MazeMapController { get; set; }
    }

    public class GameModel : AbstractModel, IGameModel
    {
        
        
        
        
        
        
        
        protected override void OnInit()
        {
        }

        public IGameData GameData { get; set; }
        public IMazeMapController MazeMapController { get; set; }
    }
}