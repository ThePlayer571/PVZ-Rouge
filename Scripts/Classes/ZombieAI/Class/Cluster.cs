using System;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.Class
{
    public class Cluster
    {
        public Vertex vertexA { get; }
        public Vertex vertexB { get; }


        public Cluster(Vertex vertexA, Vertex vertexB)
        {
            this.vertexA = vertexA;
            this.vertexB = vertexB;
        }

        public bool Include(Vertex vertex)
        {
            return vertex == vertexA || vertex == vertexB;
        }

        public bool IsIdentical(Cluster other)
        {
            return (vertexA == other.vertexA && vertexB == other.vertexB) ||
                   (vertexA == other.vertexB && vertexB == other.vertexA);
        }

        public bool GetIntersection(Cluster other, out Vertex intersection)
        {
            intersection = null;

            if (vertexA == other.vertexA || vertexA == other.vertexB)
            {
                intersection = vertexA;
                return true;
            }

            if (vertexB == other.vertexA || vertexB == other.vertexB)
            {
                intersection = vertexB;
                return true;
            }

            return false;
        }
        
        public override int GetHashCode()
        {
            var hashCodeA = this.vertexA.GetHashCode();
            var hashCodeB = this.vertexB.GetHashCode();
            return HashCode.Combine(
                Math.Min(hashCodeA, hashCodeB),
                Math.Max(hashCodeA, hashCodeB)
            );
        }

        public override bool Equals(object obj)
        {
            if (obj is Cluster other)
            {
                return (Equals(this.vertexA, other.vertexA) && Equals(this.vertexB, other.vertexB)) ||
                       (Equals(this.vertexA, other.vertexB) && Equals(this.vertexB, other.vertexA));
            }

            throw new ArgumentException("与不允许的类型相比");
        }
    }
}
