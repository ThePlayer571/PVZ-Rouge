using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace TPL.PVZR
{
    [CreateAssetMenu(fileName = "CardData_", menuName = "PVZR/CardData", order = 0)]
    public class CardDataSO:ScriptableObject
    {
        public SeedDataSO seedData;
    }

    public static class CardData
    {
        private static Dictionary<PlantIdentifier, CardDataSO> CardDataDict;
        public static CardDataSO GetCardData(PlantIdentifier plantIdentifier)
        {
            return CardDataDict.TryGetValue(plantIdentifier, out var data)
                ? data
                : CardDataDict[PlantIdentifier.PeaShooter];
        }

        static CardData()
        {
            var _ResLoader = ResLoader.Allocate();
            CardDataDict = new Dictionary<PlantIdentifier, CardDataSO>
            {
                [PlantIdentifier.PeaShooter] = _ResLoader.LoadSync<CardDataSO>("CardData_PeaShooter"),
                [PlantIdentifier.Sunflower] = _ResLoader.LoadSync<CardDataSO>("CardData_Sunflower"),
                [PlantIdentifier.Wallnut] = _ResLoader.LoadSync<CardDataSO>("CardData_Wallnut"),
                [PlantIdentifier.Flowerpot] = _ResLoader.LoadSync<CardDataSO>("CardData_Flowerpot"),
                [PlantIdentifier.SnowPea] = _ResLoader.LoadSync<CardDataSO>("CardData_SnowPea"),
                [PlantIdentifier.CherryBoom] = _ResLoader.LoadSync<CardDataSO>("CardData_CherryBoom"),
                [PlantIdentifier.PotatoMine] = _ResLoader.LoadSync<CardDataSO>("CardData_PotatoMine"),
            };
            
        }
    }
}