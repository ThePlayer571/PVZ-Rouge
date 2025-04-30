using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Gameplay.Data;
using TPL.PVZR.Gameplay.Entities;
using TPL.PVZR.Gameplay.ViewControllers.UI;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.Items
{
    public static class CardHelper
    {
        static CardHelper()
        {
            ResKit.Init();
            _ResLoader = ResLoader.Allocate();
            // 初始化操作
            _CardDataDict = new Dictionary<PlantIdentifier, CardSO>
            {
                [PlantIdentifier.PeaShooter] = _ResLoader.LoadSync<CardSO>(Carddata_peashooter_asset.BundleName,
                    Carddata_peashooter_asset.CardData_PeaShooter),
                [PlantIdentifier.Sunflower] = _ResLoader.LoadSync<CardSO>(Carddata_sunflower_asset.BundleName,
                    Carddata_sunflower_asset.CardData_Sunflower),
                [PlantIdentifier.Wallnut] = _ResLoader.LoadSync<CardSO>(Carddata_wallnut_asset.BundleName,
                    Carddata_wallnut_asset.CardData_Wallnut),
                [PlantIdentifier.Flowerpot] = _ResLoader.LoadSync<CardSO>(Carddata_flowerpot_asset.BundleName,
                    Carddata_flowerpot_asset.CardData_Flowerpot),
                [PlantIdentifier.SnowPea] = _ResLoader.LoadSync<CardSO>(Carddata_snowpea_asset.BundleName,
                    Carddata_snowpea_asset.CardData_SnowPea),
                [PlantIdentifier.CherryBoom] = _ResLoader.LoadSync<CardSO>(Carddata_cherryboom_asset.BundleName,
                    Carddata_cherryboom_asset.CardData_CherryBoom),
                [PlantIdentifier.PotatoMine] = _ResLoader.LoadSync<CardSO>(Carddata_potatomine_asset.BundleName,
                    Carddata_potatomine_asset.CardData_PotatoMine),
                [PlantIdentifier.Blover] = _ResLoader.LoadSync<CardSO>(Carddata_blover_asset.BundleName,
                    Carddata_blover_asset.CardData_Blover),
                [PlantIdentifier.CabbagePult] = _ResLoader.LoadSync<CardSO>(Carddata_cabbagepult_asset.BundleName,
                    Carddata_cabbagepult_asset.CardData_CabbagePult),
                [PlantIdentifier.Cactus] = _ResLoader.LoadSync<CardSO>(Carddata_cactus_asset.BundleName,
                    Carddata_cactus_asset.CardData_Cactus),
                [PlantIdentifier.Caltrop] = _ResLoader.LoadSync<CardSO>(Carddata_caltrop_asset.BundleName,
                    Carddata_caltrop_asset.CardData_Caltrop),
                [PlantIdentifier.Chomper] = _ResLoader.LoadSync<CardSO>(Carddata_chomper_asset.BundleName,
                    Carddata_chomper_asset.CardData_Chomper),
                [PlantIdentifier.CornPult] = _ResLoader.LoadSync<CardSO>(Carddata_cornpult_asset.BundleName,
                    Carddata_cornpult_asset.CardData_CornPult),
                [PlantIdentifier.MelonPult] = _ResLoader.LoadSync<CardSO>(Carddata_melonpult_asset.BundleName,
                    Carddata_melonpult_asset.CardData_MelonPult),
                [PlantIdentifier.RepeaterPea] = _ResLoader.LoadSync<CardSO>(Carddata_repeaterpea_asset.BundleName,
                    Carddata_repeaterpea_asset.CardData_RepeaterPea),
                [PlantIdentifier.SplitPea] = _ResLoader.LoadSync<CardSO>(Carddata_splitpea_asset.BundleName,
                    Carddata_splitpea_asset.CardData_SplitPea),
            };
        }


        #region 静态公有方法

        #region 获取数据文件

        /// <summary>
        /// 获取指定植物的卡牌数据
        /// </summary>
        /// <param name="plantIdentifier"></param>
        /// <returns></returns>
        public static CardSO GetCardSO(PlantIdentifier plantIdentifier)
        {
            return _CardDataDict.TryGetValue(plantIdentifier, out var foundCardSO)
                ? foundCardSO
                : _CardDataDict[PlantIdentifier.PeaShooter];
        }

        #endregion

        # region 创建GameObject

        public static Card CreateCard(CardSO cardSO)
        {
            return new Card(cardSO);
        }

        public static Card CreateCard(PlantIdentifier plantIdentifier)
        {
            return new Card(GetCardSO(plantIdentifier));
        }

        public static GameObject CreateSeed(SeedSO seedSO)
        {
            var go = _ResLoader.LoadSync<GameObject>("Seed").Instantiate();
            go.GetComponent<Seed>().Init(seedSO);
            return go;
        }

        public static UICard CreateUICard(Card card)
        {
            UICard go = _ResLoader.LoadSync<GameObject>("Card").Instantiate().GetComponent<UICard>();
            go.Init(card);
            return go;
        }

        # endregion

        #endregion

        # region 私有

        private static readonly Dictionary<PlantIdentifier, CardSO> _CardDataDict;
        private static readonly ResLoader _ResLoader;

        # endregion
    }
}