using UnityEngine;

namespace TPL.PVZR
{
    [CreateAssetMenu(fileName = "Card_", menuName = "PVZR/CardData", order = 0)]
    public class CardSO : ScriptableObject
    {
        public PlantIdentifier plantIdentifier;
        public bool haveInitialColdTime;
        public float coldTime;
        public int sunpointCost;
        public Sprite followingSprite;
    }
}