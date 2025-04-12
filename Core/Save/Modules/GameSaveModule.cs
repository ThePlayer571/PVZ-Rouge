using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Gameplay.Class.MazeMap;
using TPL.PVZR.Gameplay.Class.MazeMap.Core;

namespace TPL.PVZR.Core.Save.Modules
{
    public interface ISaveData : ICloneable
    {
    }

    public class GameSaveData : ISaveData
    {
        #region 数据结构

        public DeterministicRandom.State gameRandomState;
        public MazeMapSaveData mazeMapSaveData = new();

        #endregion

        public object Clone()
        {
            return new GameSaveData
            {
                gameRandomState = this.gameRandomState,
                mazeMapSaveData = this.mazeMapSaveData.Clone() as MazeMapSaveData
            };
        }
    }

    /// <summary>
    /// 地图数据结构
    /// </summary>
    /// <remarks>也是MazeMapCreateData</remarks>>
    public class MazeMapSaveData : ICloneable
    {
        #region 数据结构

        public MazeMapIdentifier identifier;
        public ulong seed;

        /// <summary>
        /// 玩家通过的Spot路径，按照从前到后的顺序排列
        /// </summary>
        public List<int> passSpotIds = new();

        #endregion

        public object Clone()
        {
            return new MazeMapSaveData
                { identifier = this.identifier, seed = this.seed, passSpotIds = this.passSpotIds.ToList() };
        }
    }

    public class GameSaveModule : ISaveModule
    {
        public string ModuleKey { get; } = "game";

        private GameSaveData _data;

        /// <summary>
        /// 读取存储数据（使用深拷贝）
        /// </summary>
        /// <returns></returns>
        public object GetData()
        {
            return _data.Clone();
        }

        /// <summary>
        /// 设置存储数据（使用深拷贝）
        /// </summary>
        /// <param name="data"></param>
        public void SetData(MazeMapSaveData data)
        {
            _data.mazeMapSaveData = data.Clone() as MazeMapSaveData;
        }

        public string Save()
        {
            return JsonConvert.SerializeObject(_data);
        }

        public void Load(string data)
        {
            _data = JsonConvert.DeserializeObject<GameSaveData>(data);
        }

        public void Reset()
        {
            _data = new GameSaveData();
        }
    }
}