using System;
using TPL.PVZR.Classes.DataClasses.Level;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Tomb
{
    public interface ITombData
    {
        Vector2Int Position { get; }
        LevelDefinition LevelDefinition { get; }
    }

    public class TombData : ITombData, IEquatable<TombData>
    {
        public TombData(Vector2Int pos, LevelDefinition levelDefinition)
        {
            Position = pos;
            LevelDefinition = levelDefinition;
        }

        public Vector2Int Position { get; }
        public LevelDefinition LevelDefinition { get; }
        
        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TombData);
        }

        public bool Equals(TombData other)
        {
            return other != null && Position.Equals(other.Position);
        }

        public static bool operator ==(TombData left, TombData right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(TombData left, TombData right)
        {
            return !(left == right);
        }
    }
}