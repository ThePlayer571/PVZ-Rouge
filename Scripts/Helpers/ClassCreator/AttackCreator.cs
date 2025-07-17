using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Helpers.New.DataReader;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class AttackCreator
    {
        public static AttackData CreateAttackData(AttackId id)
        {
            var definition = AttackConfigReader.GetAttackDefinition(id);
            return new AttackData(definition);
        }
    }
}