using System;
using System.Collections.Generic;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit
{
    public interface IPath
    {
        void Add(IKeyEdge edge);
        List<IKeyEdge> keyEdges { get; }
    }

    public class Path : IPath
    {
        #region 数据结构

        public List<IKeyEdge> keyEdges { get; }

        #endregion

        #region 公有方法

        public float Weight(AITendency aiTendency)
        {
            throw new NotImplementedException();
        }

        public void Add(IKeyEdge edge)
        {
            keyEdges.Add(edge);
        }

        #endregion

        #region 构造函数

        public Path(IKeyEdge startKeyEdge, Path path, IKeyEdge endKeyEdge)
        {
            keyEdges = new List<IKeyEdge>();
            this.keyEdges.Add(startKeyEdge);
            this.keyEdges.AddRange(path.keyEdges);
            this.keyEdges.Add(endKeyEdge);
        }

        public Path()
        {
            keyEdges = new List<IKeyEdge>();
        }

        #endregion
    }
}