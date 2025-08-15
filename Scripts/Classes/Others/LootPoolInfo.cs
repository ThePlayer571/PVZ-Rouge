using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Tools;
using UnityEngine;

namespace TPL.PVZR.Classes.LootPool
{
    
    [Serializable]
    public class LootPoolInfo : IGenerateInfo<LootPoolInfo>
    {
        public float weight;
        public LootPoolDef lootPoolDef;
        
        public List<PlantId> cards;
        public List<PlantBookId> plantBooks;
        public List<Vector2Int> coinRanges;
        public bool seedSlot;

        public float Value => 0;
        public float Weight => weight;
        public LootPoolInfo Output => this;
    }

    [Serializable]
    public struct LootPoolDef
    {
        public LootPoolId Id;
    }

    [Serializable]
    public enum LootPoolId
    {
        NotSet = 0,
        General = 1,

        Dave = 101,
        Dungeon = 102,

        PeaFamily = 201,
        PultFamily = 202,
    }
}