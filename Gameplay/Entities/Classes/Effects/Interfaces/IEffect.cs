using System.Collections.Generic;
using TPL.PVZR.Gameplay.Class.Tags;

namespace TPL.PVZR.Gameplay.Class.Effects
{
    public interface IEffect
    {
        /// <summary>
        /// 本个Effect的标识符
        /// </summary>
        EffectId effectId { get; }
        /// <summary>
        /// (运行时的)持续时间
        /// </summary>
        float duration { get; set; }
        
        
    }

}