using UnityEngine;

namespace TPL.PVZR.Gameplay.Data
{
    [CreateAssetMenu(fileName = "ArmorData_", menuName = "PVZR/ArmorData", order = 1)]
    public class ArmorData : ScriptableObject
    {
        public float duration;
        public float damagedDuration;
    }
}