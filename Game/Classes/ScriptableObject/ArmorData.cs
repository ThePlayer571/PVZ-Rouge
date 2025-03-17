using UnityEngine;
using QFramework;

namespace TPL.PVZR.EntityZombie
{
    [CreateAssetMenu(fileName = "ArmorData_", menuName = "PVZR/ArmorData", order = 1)]
    public class ArmorData : ScriptableObject
    {
        public float duration;
        public float damagedDuration;
    }
}