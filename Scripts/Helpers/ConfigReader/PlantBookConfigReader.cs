using System.Collections.Generic;
using System.Threading.Tasks;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.ConfigLists;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class PlantBookConfigReader
    {
        #region 数据存储

        private static Dictionary<PlantBookId, PlantBookDefinition> _plantBookDefinitionDict;

        public static async Task InitializeAsync()
        {
            var handel = Addressables.LoadAssetsAsync<PlantBookDefinition>("PlantBookDefinition", null);
            await handel.Task;
            _plantBookDefinitionDict = new Dictionary<PlantBookId, PlantBookDefinition>();
            foreach (var definition in handel.Result)
            {
                _plantBookDefinitionDict.Add(definition.Id, definition);
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