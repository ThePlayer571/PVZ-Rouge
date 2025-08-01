using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFramework;

namespace TPL.PVZR.Classes.DataClasses_InLevel.Effect
{
    /// <summary>
    /// 负责：Effect添加与删除的管理
    /// 不负责：Effect具体效果的实现
    /// </summary>
    public class EffectGroup : IEnumerable<EffectData>
    {
        public readonly EasyEvent<EffectData> OnEffectAdded = new EasyEvent<EffectData>();
        public readonly EasyEvent<EffectData> OnEffectRemoved = new EasyEvent<EffectData>();

        private List<EffectData> _effects { get; set; } = new();

        private void RemoveEffect(EffectData effectData)
        {
            _effects.Remove(effectData);
            OnEffectRemoved.Trigger(effectData);
        }

        public bool ContainsEffect(EffectId effectId)
        {
            return _effects.Any(data => data.effectId == effectId);
        }

        public bool ContainsEffectOR(params EffectId[] effectIds)
        {
            return effectIds.Any(id => _effects.Any(effect => effect.effectId == id));
        }
        
        public bool ContainsEffectAND(params EffectId[] effectIds)
        {
            return effectIds.All(id => _effects.Any(effect => effect.effectId == id));
        }

        public void GiveEffect(EffectData effectData)
        {
            // 可覆盖的效果
            var existingEffect = _effects.FirstOrDefault(oldEffectData =>
                (oldEffectData.effectId == effectData.effectId));
            if (existingEffect == null)
            {
                _effects.Add(effectData);
                OnEffectAdded.Trigger(effectData);
            }
            else if (
                (effectData.level > existingEffect.level) ||
                (effectData.level == existingEffect.level &&
                 effectData.timer.Remaining > existingEffect.timer.Remaining)
            )
            {
                _effects.Remove(existingEffect);
                OnEffectRemoved.Trigger(existingEffect);
                _effects.Add(effectData);
                OnEffectAdded.Trigger(effectData);
            }
        }

        public void RemoveEffect(params EffectId[] effectIds)
        {
            for (int i = 0; i < _effects.Count; i++)
            {
                var effectData = _effects[i];
                if (effectIds.Any(id => id == effectData.effectId))
                {
                    RemoveEffect(effectData);
                    i--;
                }
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
                RemoveEffect(effectData);
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