using QFramework;
using TPL.PVZR.Gameplay.Class.Games;
using TPL.PVZR.Gameplay.Class.MazeMap;
using TPL.PVZR.Gameplay.Class.MazeMap.Core;

namespace TPL.PVZR.Architecture.Models
{
    public interface IGameModel : IModel
    {
        /// <summary>
        /// 当前正在运行的游戏
        /// </summary>
        IGame currentGame { get; }

        /// <summary>
        /// 设置currentGame
        /// </summary>
        /// <param name="game"></param>
        void SetCurrentGame(IGame game);

        /// <summary>
        /// 玩家进入了enterNode，正在体验对应Spot的游戏内容
        /// </summary>
        Node lastEnteredNode { get; }

        void SetLastEnteredNode(Node enterNode);

        //
        IMazeMap currentMazeMap { get; set; }
        void SetCurrentMazeMap(IMazeMap mazeMap);
    }

    public class GameModel : AbstractModel, IGameModel
    {
        # region 私有

        protected override void OnInit()
        {
        }

        #endregion

        #region IGameModel

        public IGame currentGame { get; set; } = null;

        public void SetCurrentGame(IGame game)
        {
            currentGame = game;
        }

        public Node lastEnteredNode { get; private set; }

        public void SetLastEnteredNode(Node enterNode)
        {
            this.lastEnteredNode = enterNode;
        }

        public IMazeMap currentMazeMap { get; set; } = null;

        public void SetCurrentMazeMap(IMazeMap mazeMap)
        {
            "callsetcyrr".LogInfo();
            currentMazeMap = mazeMap;
        }

        #endregion
    }
}