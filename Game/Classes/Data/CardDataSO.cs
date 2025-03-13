using UnityEngine;

namespace TPL.PVZR
{
    [CreateAssetMenu(fileName = "CardData_", menuName = "PVZR/CardData", order = 0)]
    public class CardDataSO:ScriptableObject
    {
        public bool isGolden;
        public PlantIdentifier plant;
        public Sprite sprite;
        public SeedDataSO seedData;
    }
}