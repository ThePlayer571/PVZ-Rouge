using System;
using System.Collections.Generic;
using TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.New.Others
{
    public interface IPathCache
    {
        bool TryGetValue(PathKey pathKey, out List<Path> paths);
        bool ContainsPath(PathKey pathKey);
        void AddPath(PathKey pathKey, Path path);
    }


    public class PathCache: IPathCache
    {
        #region IPathCache
        
        public bool TryGetValue(PathKey pathKey, out List<Path> paths)
        {
            return _pathDict.TryGetValue(pathKey, out paths);
        }

        public bool ContainsPath(PathKey pathKey)
        {
            return _pathDict.ContainsKey(pathKey);
        }

        public void AddPath(PathKey pathKey, Path path)
        {
            throw new NotImplementedException();
        }

        #endregion
        
        private Dictionary<PathKey, List<Path>> _pathDict;

    }

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
            throw new Exception("Object is not a PathKey");
        }
    }
}