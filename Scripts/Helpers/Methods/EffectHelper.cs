using UnityEngine;

namespace TPL.PVZR.Helpers.New.Methods
{
    public static class EffectHelper
    {
        public static float Zombie_Chill_AttackFactor(float level)
        {
            return Mathf.Clamp(1 - 0.1f * level, 0, 1);
        }
        public static float Zombie_Chill_SpeedFactor(float level)
        {
            return Mathf.Clamp(1 - 0.1f * level, 0, 1);
        }
    }
}