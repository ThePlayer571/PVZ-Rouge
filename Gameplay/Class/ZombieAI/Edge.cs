namespace TPL.PVZR.Gameplay.Class.ZombieAI
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
    }

    public class Edge: IEdge
    {
        public enum EdgeType
        {
            WalkJump,
            Fall,
            Jump,
            HumanLadder,
            Climb
        }

        public Vertex From{ get; }
        public Vertex To{ get; }
        public EdgeType edgeType{ get; }
        public AllowedPassHeight allowedPassHeight{ get; }

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