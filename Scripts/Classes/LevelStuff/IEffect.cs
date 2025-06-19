using QFramework;

namespace TPL.PVZR.Classes.LevelStuff
{
    public interface IEffect
    {
        
    }

    public class Effect
    {
        public readonly EasyEvent OnEffectStart = new EasyEvent();
        public readonly EasyEvent OnEffectEnd = new EasyEvent();
    }
}