using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TPL.PVZR.Classes.DataClasses.Effect
{
    public class EffectGroup : IEnumerable<EffectData>
    {
        private List<EffectData> _effects { get; set; } = new();

        public void GiveEffect(EffectData effectData)
        {
            // 可覆盖的效果
            var existingEffect = _effects.FirstOrDefault(oldEffectData =>
                (oldEffectData.effectId == effectData.effectId));
            if (existingEffect == null)
            {
                _effects.Add(effectData);
            }
            else if (
                (effectData.level > existingEffect.level) ||
                (effectData.level == existingEffect.level &&
                 effectData.timer.Remaining > existingEffect.timer.Remaining)
            )
            {
                _effects.Remove(existingEffect);
                _effects.Add(effectData);
            }
        }

        public void Update(float deltaTime)
        {
            List<EffectData> toRemove = new List<EffectData>();
            foreach (var effectData in _effects)
            {
                effectData.Update(deltaTime);
                if (effectData.timer.Ready)
                {
                    toRemove.Add(effectData);
                }
            }

            foreach (var effectData in toRemove)
            {
                _effects.Remove(effectData);
            }
        }

        public IEnumerator<EffectData> GetEnumerator()
        {
            return _effects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}