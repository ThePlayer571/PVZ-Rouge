using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.Effects
{
    public abstract class Effect : IEffect
    {
        public EffectId effectId { get; protected set; }
        public float duration { get; set; }
    }

    /// <summary>
    /// 未来Effect可能会写方法，GeneralEffect是不需要复杂编辑的Effect，用着方便
    /// </summary>
    public class GeneralEffect : Effect
    {
        public GeneralEffect(EffectId effectId, float duration)
        {
            this.effectId = effectId;
            this.duration = duration;
        }
    }
}