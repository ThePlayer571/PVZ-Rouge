using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Effect;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Attack
{
    public enum PunchType
    {
        Free, ConstrainHorizontal
    }
    
    [CreateAssetMenu(fileName = "AttackDefinition_", menuName = "PVZR/AttackDefinition", order = 2)]
    public class AttackDefinition : ScriptableObject
    {
        [SerializeField] public float damage;
        [SerializeField] public float punchForce;
        [SerializeField] public PunchType punchType;
        [SerializeField] public bool isFrameDamage;
        [SerializeField] public List<EffectDefinition> effects;
    }
}