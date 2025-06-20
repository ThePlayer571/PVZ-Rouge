using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Classes.Game
{
    public class GameData : IGameData
    {
        public IMazeMapData MazeMapData { get; set; }


        //

        public GameData(MazeMapData mazeMapData)
        {
            this.MazeMapData = mazeMapData;
        }
    }
}