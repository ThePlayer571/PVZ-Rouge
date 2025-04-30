namespace TPL.PVZR.Gameplay.Class.ZombieAI
{
    public class Edge
    {
        public enum EdgeType
        {
            WalkJump, Fall, Jump, HumanLadder, Climb 
        }
        public Vertex From;
        public Vertex To;
        public EdgeType edgeType;
        public AllowPassHeight allowPassHeight;

        public Edge(Vertex from, Vertex to, EdgeType edgeType, AllowPassHeight allowPassHeight = AllowPassHeight.NotSet)
        {
            this.From = from;
            this.To = to;
            this.edgeType = edgeType;
            this.allowPassHeight = allowPassHeight;
        }
    }
}