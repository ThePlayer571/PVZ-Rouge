using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities;
using TPL.PVZR.Gameplay.ViewControllers.UI;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.Items
{

    public class Card : Item, IComparable<Card>
    {
        # region 数据存储
        public CardSO cardSO { get;private set; }
        #endregion
        
        # region 比较方法

        public int CompareTo(Card other)
        {
            int plantIdCompare = cardSO.seedSO.plantIdentifier.CompareTo(other.cardSO.seedSO.plantIdentifier);
            int itemIdCompare = this.id.CompareTo(other.id);
            if (plantIdCompare != 0) return plantIdCompare;
            if (itemIdCompare != 0) return itemIdCompare;
            return 0;
        }
        #endregion
        
        # region 静态公有方法

        #region 获取数据文件
/// <summary>
/// 获取指定植物的卡牌数据
/// </summary>
/// <param name="plantIdentifier"></param>
/// <returns></returns>
        public static CardSO GetCardSO(PlantIdentifier plantIdentifier)
        {
            return CardDataDict.TryGetValue(plantIdentifier, out var foundCardSO)
                ? foundCardSO
                : CardDataDict[PlantIdentifier.PeaShooter];
        }
        

        #endregion
        
        # region 创建GameObject
        
        public static Card CreateCard(CardSO cardSO)
        {
            return new Card { cardSO = cardSO};
        }
        public static Card CreateCard(PlantIdentifier plantIdentifier)
        {
            return new Card { cardSO = GetCardSO(plantIdentifier)};
        }
        public static GameObject CreateSeed(SeedSO seedSO)
        {
            var _ResLoader = ResLoader.Allocate();
            var go = _ResLoader.LoadSync<GameObject>("Seed").Instantiate();
            go.GetComponent<Seed>().Init(seedSO);
            return go;
        }
        public static UICard CreateUICard(Card card)
        {
            var _ResLoader = ResLoader.Allocate();
            UICard go = _ResLoader.LoadSync<GameObject>("Card").Instantiate().GetComponent<UICard>();
            go.Init(card);
            return go;
        }
        
        # endregion
        
        #endregion
        
        # region 私有
        private static readonly Dictionary<PlantIdentifier, CardSO> CardDataDict;
        static Card()
        {
            var _ResLoader = ResLoader.Allocate();
            CardDataDict = new Dictionary<PlantIdentifier, CardSO>
            {
                [PlantIdentifier.PeaShooter] = _ResLoader.LoadSync<CardSO>("CardData_PeaShooter"),
                [PlantIdentifier.Sunflower] = _ResLoader.LoadSync<CardSO>("CardData_Sunflower"),
                [PlantIdentifier.Wallnut] = _ResLoader.LoadSync<CardSO>("CardData_Wallnut"),
                [PlantIdentifier.Flowerpot] = _ResLoader.LoadSync<CardSO>("CardData_Flowerpot"),
                [PlantIdentifier.SnowPea] = _ResLoader.LoadSync<CardSO>("CardData_SnowPea"),
                [PlantIdentifier.CherryBoom] = _ResLoader.LoadSync<CardSO>("CardData_CherryBoom"),
                [PlantIdentifier.PotatoMine] = _ResLoader.LoadSync<CardSO>("CardData_PotatoMine"),
            };
            
        }

        private Card()
        {
            
        }
        # endregion
    }
}