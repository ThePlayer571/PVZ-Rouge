using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Tools.Random;
using UnityEngine;

namespace TPL.PVZR.Tools.Save
{
    public class GameSaveData : ISaveData
    {
        public MazeMapSaveData mazeMapSaveData;
        public InventorySaveData inventorySaveData;
        public GlobalEntityData globalEntityData;
        public ulong seed;
        public DeterministicRandom.State randomState;
    }

    public class InventorySaveData : ISaveData
    {
        public int initialSunpoint;
        public int seedSlotCount;
        public int coins;
        public List<CardSaveData> cards;
        public List<PlantBookSaveData> plantBooks;
    }

    public class CardSaveData : ISaveData
    {
        public PlantDef plantDef;
        public bool locked;
    }

    public class PlantBookSaveData : ISaveData
    {
        public PlantBookId plantBookId;
    }


    public class MazeMapSaveData : ISaveData
    {
        public MazeMapDef mazeMapDef;
        public ulong generateSeed;
        public List<TombSaveData> discoveredTombs;
        public List<Vector2IntSerializable> passedRoute;
    }

    public class TombSaveData : ISaveData
    {
        public int stage;
        public int positionX;
        public int positionY;
        public LevelDef levelDef;
    }

    [Serializable]
    public struct Vector2IntSerializable
    {
        public int x;
        public int y;

        public Vector2IntSerializable(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2IntSerializable(Vector2Int v)
        {
            this.x = v.x;
            this.y = v.y;
        }
    }
}