using System;
using System.Runtime.CompilerServices;
using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Core;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Core.Save;
using TPL.PVZR.Core.Save.Modules;
using TPL.PVZR.Gameplay.Class.MazeMap;
using TPL.PVZR.Gameplay.Class.MazeMap.Core;

namespace TPL.PVZR.Gameplay.Class.Games
{
    
    public interface IGame
    {
        MazeMapSaveData mazeMapSaveData { get; }
    }
    
    
    // 进入游戏时用到的数据结构
    public class Game : IGame,ICanGetSystem
    {
        # region 数据结构
        public MazeMapSaveData mazeMapSaveData { get; protected set; }

        # endregion

        #region 公有方法

        public GameSaveData ToGameSaveData()
        {
            throw new NotImplementedException();
        }
        

        #endregion
        
        #region 工厂模式

        public static IGame CreateRandom(ulong? seed = null)
        {
            seed ??= RandomHelper.Default.NextUnsigned();
            Game game = new()
            {
                mazeMapSaveData = new MazeMapSaveData
                    { seed = seed.Value, identifier = MazeMapIdentifier.DaveLawn }
            };
            return game;
        }

        public static IGame CreateLoad()
        {
            throw new NotImplementedException();
        }
        

        #endregion

        #region 私有

        private Game()
        {
            
        }
        

        #endregion
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }

}