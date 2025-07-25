using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.DataClasses.Recipe;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class AttackConfigReader
    {
        #region 数据存储

        private static readonly Dictionary<AttackId, AttackDefinition> _attackDefinitionDict;

        static AttackConfigReader()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();

            var attackDefinitionList = resLoader
                .LoadSync<AttackDefinitionList>(Configlist.BundleName, Configlist.AttackDefinitionList)
                .attackDefinitionList;
            _attackDefinitionDict = new Dictionary<AttackId, AttackDefinition>();
            foreach (var attackDefinition in attackDefinitionList)
            {
                _attackDefinitionDict.Add(attackDefinition.attackId, attackDefinition);
            }
            
            resLoader.Recycle2Cache();
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