using TPL.PVZR.Classes.LevelStuff;

namespace TPL.PVZR.Helpers.Factory
{
    public static class AttackHelper
    {
        public static AttackData CreateAttackData(AttackDefinition attackDefinition)
        {
            if (attackDefinition == null)
                throw new System.ArgumentNullException(nameof(attackDefinition), "AttackDefinition cannot be null");

            return new AttackData(attackDefinition);
        }
    }
}