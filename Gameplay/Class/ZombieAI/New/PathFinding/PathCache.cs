using System;
using System.Collections.Generic;
using TPL.PVZR.Gameplay.Class.ZombieAI.New.Class;
using TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.New.Others
{
    public interface IPathCache
    {
        bool TryGetValue(PathKey pathKey, out List<IPath> paths);
        bool ContainsPath(PathKey pathKey);
        void AddPath(PathKey pathKey, IPath path);
        void AddPathRange(PathKey pathKey, IEnumerable<IPath> paths);
    }

    public class PathCache : IPathCache
    {
        #region IPathCache

        public bool TryGetValue(PathKey pathKey, out List<IPath> paths)
        {
            return _pathDict.TryGetValue(pathKey, out paths);
        }

        public bool ContainsPath(PathKey pathKey)
        {
            return _pathDict.ContainsKey(pathKey);
        }

        public void AddPath(PathKey pathKey, IPath path)
        {
            if (!_pathDict.TryGetValue(pathKey, out List<IPath> pathList))
            {
                pathList = new List<IPath>();
                _pathDict[pathKey] = pathList;
            }
            pathList.Add(path);
        }

        public void AddPathRange(PathKey pathKey, IEnumerable<IPath> paths)
        {
            if (!_pathDict.TryGetValue(pathKey, out List<IPath> pathList))
            {
                pathList = new List<IPath>();
                _pathDict[pathKey] = pathList;
            }
            pathList.AddRange(paths);
        }

        #endregion

        private readonly Dictionary<PathKey, List<IPath>> _pathDict = new Dictionary<PathKey, List<IPath>>();
    }

    
}
