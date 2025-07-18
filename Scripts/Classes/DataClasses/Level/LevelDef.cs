using System;
using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Classes.DataClasses.Level
{
    [Serializable]
    public struct LevelDef : IEquatable<LevelDef>
    {
        public LevelId Id;
        public GameDifficulty Difficulty;

        public override string ToString()
        {
            return $"LevelDef(Id: {Id}, Difficulty: {Difficulty})";
        }

        public bool Equals(LevelDef other)
        {
            return Id == other.Id && Difficulty == other.Difficulty;
        }

        public override bool Equals(object obj)
        {
            return obj is LevelDef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Difficulty);
        }

        public static bool operator ==(LevelDef left, LevelDef right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LevelDef left, LevelDef right)
        {
            return !left.Equals(right);
        }
    }
}