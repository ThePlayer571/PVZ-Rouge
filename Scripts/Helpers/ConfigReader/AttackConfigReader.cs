using System.Collections.Generic;
using System.Threading.Tasks;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.DataClasses.Recipe;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class AttackConfigReader
    {
        #region 数据存储

        private static Dictionary<AttackId, AttackDefinition> _attackDefinitionDict;

        public static async Task InitializeAsync()
        {
            var handle = Addressables.LoadAssetsAsync<AttackDefinition>("AttackDefinition", null);
            await handle.Task;
            _attackDefinitionDict = new Dictionary<AttackId, AttackDefinition>();
            foreach (var attackDefinition in handle.Result)
            {
                _attackDefinitionDict.Add(attackDefinition.attackId, attackDefinition);
            }

            handle.Release();
        }

        #endregion

        #region 数据读取

        public static AttackDefinition GetAttackDefinition(AttackId id)
        {
            if (_attackDefinitionDict.TryGetValue(id, out var definition))
            {
                return definition;
            }

            throw new KeyNotFoundException($"未找到Attack: {id}");
        }

        #endregion
    }
}