using System;
using TPL.PVZR.Gameplay.Class.ZombieAI.New.Others;
using TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.New.Class
{
    public class PathKey
    {
        private readonly FromToKey<Cluster> fromToKey;
        private readonly AITendency aiTendency;

        public PathKey(FromToKey<Cluster> fromToKey, AITendency aiTendency)
        {
            this.fromToKey = fromToKey;
            this.aiTendency = aiTendency;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(fromToKey, aiTendency);
        }

        public override bool Equals(object obj)
        {
            if (obj is PathKey other)
            {
                return Equals(this.fromToKey, other.fromToKey) && Equals(this.aiTendency, other.aiTendency);
            }
            throw new ArgumentException("Object must be of type PathKey");
        }
    }
}