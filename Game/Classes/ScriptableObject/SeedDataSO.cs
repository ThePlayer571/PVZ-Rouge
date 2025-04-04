using UnityEngine;

namespace TPL.PVZR
{

    public enum CardQuality
    {
        White, Green,Blue,Purple,Gold
    }
    [CreateAssetMenu(fileName = "SeedData_", menuName = "PVZR/SeedData", order = 0)]
    public class SeedDataSO : ScriptableObject
    {
        // 游戏数据
        public PlantIdentifier plantIdentifier;
        public bool haveInitialColdTime;
        public float coldTime;
        public int sunpointCost;
        // 显示
        public Sprite followingSprite;
        public Sprite plantSprite;
        public CardQuality CardQuality;
    }
}