using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Attack;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class AttackHelper
    {
        private static Dictionary<AttackId, AttackDefinition> _attackDict;

        static AttackHelper()
        {
            var resLoader = ResLoader.Allocate();
            _attackDict = new Dictionary<AttackId, AttackDefinition>()
            {
                [AttackId.Void] = resLoader.LoadSync<AttackDefinition>(Attackdefinition.BundleName,
                    Attackdefinition.AttackDefinition_Void),
                [AttackId.Pea] = resLoader.LoadSync<AttackDefinition>(Attackdefinition.BundleName,
                    Attackdefinition.AttackDefinition_Pea),
                [AttackId.FrozenPea] = resLoader.LoadSync<AttackDefinition>(Attackdefinition.BundleName,
                    Attackdefinition.AttackDefinition_FrozenPea),
                [AttackId.NormalZombie] = resLoader.LoadSync<AttackDefinition>(Attackdefinition.BundleName,
                    Attackdefinition.AttackDefinition_NormalZombie),
                [AttackId.MungBean] = resLoader.LoadSync<AttackDefinition>(Attackdefinition.BundleName,
                    Attackdefinition.AttackDefinition_MungBean),
            };
        }

        public static AttackData CreateAttackData(AttackId id)
        {
            if (_attackDict.TryGetValue(id, out var attackDefinition))
            {
                return new AttackData(attackDefinition);
            }
            else
            {
                throw new ArgumentException($"未考虑的攻击类型: {id}");
            }
        }
    }
}