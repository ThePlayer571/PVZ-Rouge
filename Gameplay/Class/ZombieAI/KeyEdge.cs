using System;
using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit;

namespace TPL.PVZR.Gameplay.Class.ZombieAI
{
    public interface IKeyEdge : IEdge
    {
        
        float Weight(AITendency aiTendency);
        List<IEdge> includeEdges { get; }

        //
        IKeyEdge AddEdge(IEdge edge);
        IKeyEdge Adversed(Dictionary<Vertex, List<IEdge>> adjacencyList);
    }


    /// <summary>
    /// 两个keyVertex之间的连接，使用KeyEdge（比Edge多了一些数据结构）
    /// </summary>
    public class KeyEdge : IKeyEdge
    {
        public List<IEdge> includeEdges { get; private set; } = new();
        public Edge.EdgeType edgeType { get; }

        public AllowedPassHeight allowedPassHeight { get; private set; }

        //
        public Vertex From => includeEdges.First().From;
        public Vertex To => includeEdges.Last().To;
        
        /// <summary>
        /// 将KeyEdge反转（如果包含多条路，只会选择其中一条）（如果不能反转，抛出异常）
        /// </summary>
        /// <returns></returns>
        public IKeyEdge Adversed(Dictionary<Vertex, List<IEdge>> adjacencyList)
        {
            var includeEdges = new List<IEdge>();
            for (int i = includeEdges.Count - 1; i >= 0; i--)
            {
                var oldEdge = includeEdges[i];
                var newEdge = adjacencyList[oldEdge.To].Where(edge => Equals(edge.To, oldEdge.From))
                    .FirstOrDefault(null);
                if (newEdge is null) throw new Exception("该边无法反转");
                includeEdges.Add(newEdge);
            }

            this.includeEdges = includeEdges;
            return new KeyEdge(this, includeEdges);
        }

        public IKeyEdge AddEdge(IEdge edge)
        {
            if (edge.allowedPassHeight is AllowedPassHeight.One &&
                this.allowedPassHeight is AllowedPassHeight.TwoAndMore)
            {
                this.allowedPassHeight = AllowedPassHeight.One;
            }

            //
            includeEdges.Add(edge);
            return this;
        }

        public KeyEdge(IEdge startEdge)
        {
            includeEdges.Add(startEdge);
            this.edgeType = startEdge.edgeType;
            this.allowedPassHeight = startEdge.allowedPassHeight;
        }

        public KeyEdge(IKeyEdge startEdge, List<IEdge> includeEdges = null)
        {
            includeEdges ??= startEdge.includeEdges.ToList();
            
            this.includeEdges = includeEdges;
            this.edgeType = startEdge.edgeType;
            this.allowedPassHeight = startEdge.allowedPassHeight;
        }
    }
}