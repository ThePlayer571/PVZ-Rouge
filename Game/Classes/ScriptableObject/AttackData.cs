using UnityEngine;

namespace TPL.PVZR
{
    [CreateAssetMenu(fileName = "AttackData_", menuName = "PVZR/AttackData", order = 2)]
    public class AttackData:ScriptableObject
    {
        public float damage;
        public float punchForce;
        public bool slowness;
    }
}