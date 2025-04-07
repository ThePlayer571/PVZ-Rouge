using TPL.PVZR.Gameplay.Entities;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Data
{

    public enum CardQuality
    {
        White, Green,Blue,Purple,Gold
    }
    [CreateAssetMenu(fileName = "SeedData_", menuName = "PVZR/SeedData", order = 0)]
    public class SeedSO : ScriptableObject
    {
        // 游戏数据
        public PlantIdentifier plantIdentifier; // 生成的植物
        public bool haveInitialColdTime; // 有初始冷却时间
        public float coldTime; // 冷却时间
        public int sunpointCost; // 阳光消耗
        // 显示
        public Sprite followingSprite; // 跟随在手上的图片
        public Sprite plantSprite; // 卡牌上的植物图像
        public CardQuality CardQuality; // 卡牌质量
    }
}