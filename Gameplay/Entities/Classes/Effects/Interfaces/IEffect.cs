namespace TPL.PVZR.Gameplay.Entities.Classes.Effects.Interfaces
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