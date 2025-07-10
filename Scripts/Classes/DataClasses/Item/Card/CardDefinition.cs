using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Item.Card
{
    
    [CreateAssetMenu(fileName = "CardDefinition_", menuName = "PVZR/CardDefinition", order = 1)]
    public class CardDefinition : ScriptableObject
    {
        // 游戏数据
        public PlantDef PlantDef;
        public int SunpointCost;
        public float ColdTime;
        public float InitialColdTime;


        // 显示
        public Sprite PlantSprite;
        public Sprite FollowingSprite;
        public CardQuality Quality;
    }
}