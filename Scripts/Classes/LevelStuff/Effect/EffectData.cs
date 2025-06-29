namespace TPL.PVZR.Classes.LevelStuff.Effect
{
    public class EffectData
    {
        public readonly EffectId effectId;
        public readonly int level;
        public Timer timer { get; private set; }

        public void Update(float deltaTime)
        {
            timer.Update(deltaTime);
        }

        public EffectData(EffectDefinition definition)
        {
            effectId = definition.effectId;
            level = definition.level;
            timer = new Timer(definition.duration);
        }
    }
}