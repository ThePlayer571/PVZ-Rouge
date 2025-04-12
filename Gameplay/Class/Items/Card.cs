using System;
using System.Collections.Generic;
using QAssetBundle;
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

        public CardSO cardSO { get; private set; }

        #endregion

        #region 构造函数

        public Card(CardSO cardSO)
        {
            this.cardSO = cardSO;
        }

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

    }
}