using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Classes.LevelStuff
{
    public class AttackDefinition : ScriptableObject
    {
        public float damage;
        public float punchForce;
        public bool isFrameDamage;
    }
}