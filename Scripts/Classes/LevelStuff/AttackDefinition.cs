using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Classes.LevelStuff
{
    public class AttackDefinition : ScriptableObject
    {
        [SerializeField] public float damage;
        [SerializeField] public float punchForce;
        [SerializeField] public bool isFrameDamage;
        [SerializeField] public List<Effect> effects;
    }
}