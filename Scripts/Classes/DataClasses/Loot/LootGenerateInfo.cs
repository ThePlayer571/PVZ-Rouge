using System;
using TPL.PVZR.Tools;

namespace TPL.PVZR.Classes.DataClasses.Loot
{
    [Serializable]
    public class LootGenerateInfo : IGenerateInfo<LootInfo>
    {
        public float value;
        public float weight;
        public LootInfo lootInfo;

        public float Value => value;
        public float Weight => weight;
        public LootInfo Output => lootInfo;
    }
}