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
            foreach (var oldEffectData in _effects.Where(oldEffectData =>
                         (oldEffectData.effectId == effectData.effectId)
                         && ((effectData.level > oldEffectData.level) ||
                             (effectData.level == oldEffectData.level &&
                              effectData.timer.Remaining > oldEffectData.timer.Remaining))))
            {
                _effects.Remove(oldEffectData);
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