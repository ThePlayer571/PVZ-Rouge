using System;

namespace TPL.PVZR.Classes.ZombieAI.Class
{
    public class FromToKey<T> where T : class
    {
        public readonly T From;
        public readonly T To;

        public FromToKey(T from, T to)
        {
            From = from;
            To = to;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To);
        }

        public override bool Equals(object obj)
        {
            if (obj is FromToKey<T> other)
            {
                return this.From.Equals(other.From) && this.To.Equals(other.To);
            }
            
            throw new ArgumentException("与不允许的类型相比");
        }
    }
}