using System;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Effect
{
    [Serializable]
    public class EffectDefinition
    {
        [SerializeField] public EffectId effectId;
        [SerializeField] public int level;
        [SerializeField] public float duration;
    }
}