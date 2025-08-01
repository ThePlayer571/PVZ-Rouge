using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses_InLevel.ZombieArmor
{
    [CreateAssetMenu(fileName = "ZombieArmorDefinition_", menuName = "PVZR/ZombieArmorDefinition", order = 3)]
    public class ZombieArmorDefinition : ScriptableObject
    {
        [SerializeField] public ZombieArmorId zombieArmorId;
        [SerializeField] public float armorHealth;
        [SerializeField, Range(0, 1)] public float damageReductionRate = 1f;
        [SerializeField, Range(0, 1)] public float punchResistanceRate = 0.5f;
        
        [SerializeField, Range(0, 1)] public float intactMinRatio = 0.7f;
        [SerializeField, Range(0, 1)] public float damagedMinRatio = 0.45f;
        [SerializeField, Range(0, 1)] public float brokenMinRatio = 0;
    }
}