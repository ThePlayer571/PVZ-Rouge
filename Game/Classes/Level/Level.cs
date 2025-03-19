using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TPL.PVZR
{

    public interface ILevel
    {
        MapConfig MapConfig { get; }
        WaveConfig WaveConfig { get; }
        ZombieSpawnConfig ZombieSpawnConfig { get; }
        LootConfig LootConfig { get; }
    }

    public abstract class Level
    {
        public enum ZombieSpawnPositionId
        {
            Lawn, Roof
        }
    }

    
}