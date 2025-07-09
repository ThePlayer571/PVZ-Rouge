using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Item.Card
{
    public enum PlantVariant
    {
        V0, V1, V2, V3, V4, V5, V6, V7, V8, V9
    }
    
    [CreateAssetMenu(fileName = "CardDefinition_", menuName = "PVZR/CardDefinition", order = 1)]
    public class CardDefinition : ScriptableObject
    {
        // 游戏数据
        public PlantId Id;
        public PlantVariant Variant;
        public int SunpointCost;
        public float ColdTime;
        public float InitialColdTime;


        // 显示
        public Sprite PlantSprite;
        public Sprite FollowingSprite;
        public CardQuality Quality;
    }
}