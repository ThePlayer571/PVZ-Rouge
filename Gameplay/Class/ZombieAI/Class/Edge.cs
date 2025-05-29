using TPL.PVZR.Gameplay.Class.ZombieAI.Public;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.Class
{
    public interface IEdge
    {
        Vertex From { get; }
        Vertex To { get; }
        Edge.EdgeType edgeType { get; }
        /// <summary>
        /// 要通过这条边，高度一定低于这个值
        /// </summary>
        AllowedPassHeight allowedPassHeight { get; }

        float Weight(AITendency aiTendency);
    }

    public class Edge: IEdge
    {
        public enum EdgeType
        {
            WalkJump,
            Fall,
            Water,
            HumanLadder,
            ClimbLadder
        }

        private static readonly EdgeWeightCalculator WeightCalculator = new EdgeWeightCalculator();

        public Vertex From{ get; }
        public Vertex To{ get; }
        public EdgeType edgeType{ get; }
        public AllowedPassHeight allowedPassHeight{ get; }
        public float Weight(AITendency aiTendency)
        {
            return WeightCalculator.GetWeight(this.edgeType, aiTendency.mainAI);
        }

        public Edge(Vertex from, Vertex to, EdgeType edgeType,
            AllowedPassHeight allowedPassHeight = AllowedPassHeight.NotSet)
        {
            this.From = from;
            this.To = to;
            this.edgeType = edgeType;
            this.allowedPassHeight = allowedPassHeight;
        }
    }
}
