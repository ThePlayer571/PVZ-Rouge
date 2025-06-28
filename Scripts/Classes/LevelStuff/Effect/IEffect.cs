using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TPL.PVZR.Classes.LevelStuff.Effect
{
    // 半成品，因为实体的交互机制没有明晰
    // 等Entity完善了在写这个
    
    public enum EffectId
    {
    }


    public class Temp
    {
        private EffectGroup _effectGroup = new EffectGroup();
        public void Test()
        {
            Effect effect;
            
        }
    }

    public class EffectGroup
    {
        private List<Effect> _effects { get; set; } = new();
        
        public void GiveEffect(Effect effect)
        {
            if (_effects.Any(e => e.EffectId == effect.EffectId))
            {
                var existingEffect = _effects.Find(e => e.EffectId == effect.EffectId);
            }
            _effects.Add(effect);
        }
    }

    public class Effect
    {
        public EffectId EffectId { get; private set; }
        public int Level { get; set; }
        public float Duration { get;set; }


        public void Update()
        {
            Duration -= Time.deltaTime;
            if (Duration <= 0)
            {
            }
        }
    }
}