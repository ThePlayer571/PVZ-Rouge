using TPL.PVZR.Tools;

namespace TPL.PVZR.Classes.DataClasses.Effect
{
    public class EffectData
    {
        public readonly EffectId effectId;
        public readonly int level;
        public readonly float duration;
        public Timer timer { get; private set; }

        public void Update(float deltaTime)
        {
            timer.Update(deltaTime);
        }

        public EffectData(EffectDefinition definition)
        {
            effectId = definition.effectId;
            level = definition.level;
            duration = definition.duration;

            timer = new Timer(definition.duration);
        }

        public EffectData(EffectData other)
        {
            effectId = other.effectId;
            level = other.level;
            duration = other.duration;

            timer = new Timer(other.duration);
        }
    }
}