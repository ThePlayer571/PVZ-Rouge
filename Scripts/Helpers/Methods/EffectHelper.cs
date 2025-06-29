using UnityEngine;

namespace TPL.PVZR.Helpers.Methods
{
    public static class EffectHelper
    {
        public static float Zombie_Chill_Factor(float level)
        {
            return Mathf.Clamp(1 - 0.1f * level, 0, 1);
        }
    }
}