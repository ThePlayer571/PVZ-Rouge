using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Classes.LevelStuff
{
    [CreateAssetMenu(fileName = "AttackDefinition_", menuName = "PVZR/AttackDefinition", order = 2)]
    public class AttackDefinition : ScriptableObject
    {
        [SerializeField] public float damage;
        [SerializeField] public float punchForce;
        [SerializeField] public bool isFrameDamage;
    }
}