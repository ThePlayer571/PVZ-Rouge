using TPL.PVZR.Classes.ZombieAI.Public;

namespace TPL.PVZR.Classes.ZombieAI.Class
{

    public class Edge
    {
        private static readonly EdgeWeightCalculator WeightCalculator = new EdgeWeightCalculator();

        public Vertex From{ get; }
        public Vertex To{ get; }
        public MoveType moveType{ get; }
        public AllowedPassHeight allowedPassHeight{ get; }
        public float Weight(AITendency aiTendency)
        {
            return WeightCalculator.GetWeight(this.moveType, aiTendency.mainAI);
        }

        public Edge(Vertex from, Vertex to, MoveType moveType,
            AllowedPassHeight allowedPassHeight = AllowedPassHeight.NotSet)
        {
            this.From = from;
            this.To = to;
            this.moveType = moveType;
            this.allowedPassHeight = allowedPassHeight;
        }
    }
}
