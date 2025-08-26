using System;
using TPL.PVZR.Classes.ZombieAI.Public;

namespace TPL.PVZR.Classes.ZombieAI.Class
{
    public class Edge
    {
        private static readonly EdgeWeightCalculator WeightCalculator = new EdgeWeightCalculator();

        public Vertex From { get; }
        public Vertex To { get; }
        public MoveType moveType { get; }
        public int passableHeight { get; }

        public float Weight(AITendency aiTendency)
        {
            return EdgeWeightCalculator.GetWeight(this.moveType, aiTendency.mainAI, To.y - From.y);
        }

        public Edge(Vertex from, Vertex to, MoveType moveType, int passableHeight)
        {
            this.From = from;
            this.To = to;
            this.moveType = moveType;
            this.passableHeight = Math.Min(passableHeight, AITendency.PASSABLE_HEIGHT_CEILING);
        }
    }
}