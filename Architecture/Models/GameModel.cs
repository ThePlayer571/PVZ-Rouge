using QFramework;
using TPL.PVZR.Gameplay.Class.Games;
using TPL.PVZR.Gameplay.Class.MazeMap;

namespace TPL.PVZR.Architecture.Models
{
    public interface IGameModel : IModel
    {
        IGame currentGame { get; set; }
        MazeMap CurrentMazeMap { get; set; }
    }
    
    public class GameModel:AbstractModel,IGameModel
    {
        # region 私有
        protected override void OnInit()
        {
            
        }

        #endregion

        public IGame currentGame { get; set; } = null;
        public MazeMap CurrentMazeMap { get; set; } = null;
        public void SetMazeMap(MazeMap mazeMap)
        {
            CurrentMazeMap = mazeMap;
        }

    }
}