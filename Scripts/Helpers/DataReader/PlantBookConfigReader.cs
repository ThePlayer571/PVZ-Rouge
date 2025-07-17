using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.ConfigLists;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class PlantBookConfigReader
    {
        #region 数据存储

        private static readonly Dictionary<PlantBookId, PlantBookDefinition> _plantBookDefinitionDict;

        static PlantBookConfigReader()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();

            var plantBookDefinitionList =
                resLoader.LoadSync<PlantBookDefinitionList>(Configlist.BundleName, Configlist.PlantBookDefinitionList)
                    .plantBookDefinitionList;
            _plantBookDefinitionDict = new Dictionary<PlantBookId, PlantBookDefinition>();
            foreach (var config in plantBookDefinitionList)
            {
                _plantBookDefinitionDict.Add(config.plantBookId, config.plantBookDefinition);
            }
        }

        #endregion

        #region 数据读取

        public static PlantBookDefinition GetPlantBookDefinition(PlantBookId id)
        {
            if (_plantBookDefinitionDict.TryGetValue(id, out var definition))
            {
                return definition;
            }

            throw new KeyNotFoundException($"未找到PlantBook: {id}");
        }

        #endregion
    }
}