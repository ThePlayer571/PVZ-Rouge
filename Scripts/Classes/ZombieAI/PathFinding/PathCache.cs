using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Gameplay.Class.ZombieAI.Class;
using TPL.PVZR.Gameplay.Class.ZombieAI.Public;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.PathFinding
{
    public interface IPathCache
    {
        bool TryGetValue(PathKey pathKey, out List<Path> paths);
        bool ContainsPath(PathKey pathKey);
        void AddPath(PathKey pathKey, Path path);
        void AddPathRange(PathKey pathKey, IEnumerable<Path> paths);
        int Count { get; }
    }

    public class PathCache : IPathCache
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
            if (!_pathDict.TryGetValue(pathKey, out List<Path> pathList))
            {
                pathList = new List<Path>();
                _pathDict[pathKey] = pathList;
            }
            pathList.Add(path);
        }

        public void AddPathRange(PathKey pathKey, IEnumerable<Path> paths)
        {
            if (!_pathDict.TryGetValue(pathKey, out List<Path> pathList))
            {
                pathList = new List<Path>();
                _pathDict[pathKey] = pathList;
            }
            pathList.AddRange(paths);
        }

        public int Count => _pathDict.Count;

        #endregion

        private readonly Dictionary<PathKey, List<Path>> _pathDict = new Dictionary<PathKey, List<Path>>();
    }

    
}
