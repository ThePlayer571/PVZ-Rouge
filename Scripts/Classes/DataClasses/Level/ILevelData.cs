using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.ZombieSpawner;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Level
{
    public interface ILevelData
    {
        #region Others

        int InitialSunPoint { get; }
        GlobalEntityData GlobalEntityData { get; }

        #endregion

        #region Map

        Vector2Int MapSize { get; }
        Vector2 InitialPlayerPos { get; }
        GameObject LevelPrefab { get; }
        Vector2Int GetRandomSunFallCellPos();

        #endregion

        #region WaveDef

        int TotalWaveCount { get; }
        IReadOnlyList<int> HugeWaves { get; }
        bool HasFinalBoss { get; }

        #endregion

        #region WaveDifficulty

        float ValueOfWave(int wave);

        #endregion

        #region WaveDurations

        float DurationOfWave(int wave);

        #endregion

        #region ZombieSpawn

        IReadOnlyList<ZombieSpawnInfo> ZombieSpawnInfosOfWave(int wave);

        #endregion

        #region Loot

        IReadOnlyList<LootGenerateInfo> LootGenerateInfos { get; }
        float LootValue { get; }

        #endregion
    }
}