using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.Effects
{
    /*
     * 使用Combine添加效果，可以自动合并已有的效果
     * 结束的效果放到effectsToRecycle里，让类自行处理(因为不同的类对药水的处理不同)
     */
    public class EffectGroup
    {
        
        #region 公有方法

        public void Combine(IEffect effect, out bool startEffect)
        {
          IEffect targetEffect = null;
          foreach (var eachEffect in _effects)
          {
              if (eachEffect.effectId == effect.effectId)
              {
                  targetEffect = eachEffect;
                  break;
              }
          }
          //
          if (targetEffect is null)
          {
              _effects.Add(effect);
              startEffect = true;
          }
          else
          {
              targetEffect.duration = effect.duration;
              startEffect = false;
          }
        }

        // 减持续时间
        public void ReduceDuration()
        {
            foreach (var eachEffect in _effects.ToList())
            {
                eachEffect.duration -= Time.deltaTime;
                if (eachEffect.duration <= 0)
                {
                    _effects.Remove(eachEffect);
                    effectsToRecycle.Push(eachEffect);
                }
            }
        }
        
        
        #endregion

        #region 公有属性

        public Stack<IEffect> effectsToRecycle = new Stack<IEffect>();

        #endregion

        #region 构造函数

        public EffectGroup()
        {
            _effects = new List<IEffect>();
        }

        public EffectGroup(IEffect[] effects)
        {
            _effects = new List<IEffect>(effects);
        }

        #endregion

        #region 私有

        private readonly List<IEffect> _effects;

        #endregion
    }
}