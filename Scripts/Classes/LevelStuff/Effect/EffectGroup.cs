using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TPL.PVZR.Classes.LevelStuff.Effect
{
    public class EffectGroup : IEnumerable<EffectData>
    {
        private List<EffectData> _effects { get; set; } = new();

        public void GiveEffect(EffectData effectData)
        {
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