using System;
using Newtonsoft.Json;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Gameplay.Class.MazeMap;

namespace TPL.PVZR.Core.Save.Modules
{

    public interface ISaveData
    {
        
    }
    public class GameSaveData:ISaveData
    {
        public DeterministicRandom.State GameRandomState;
        public MazeMapData mazeMapData;
        
        
        
        
        //
        public struct MazeMapData
        {
            public MazeMapIdentifier mazeMapIdentifier;
            public ulong seed;
        }
    }
    
    public class GameSaveModule:ISaveModule
    {
        public string ModuleKey { get; } = "Game";

        private GameSaveData _data;


        public object GetData()
        {
            return _data;
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