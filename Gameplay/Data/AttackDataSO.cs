using UnityEngine;

namespace TPL.PVZR.Gameplay.Data
{
    [CreateAssetMenu(fileName = "AttackData_", menuName = "PVZR/AttackData", order = 2)]
    public class AttackDataSO:ScriptableObject
    {
        public float damage;
        public float punchForce;
        public bool slowness;
        public bool buttered;
        public bool isFrameDamage;
    }
}