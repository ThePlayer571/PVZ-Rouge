using System;
using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Classes.DataClasses.Level
{
    [Serializable]
    public struct LevelDef : IEquatable<LevelDef>, IComparable<LevelDef>
    {
        public LevelId Id;
        public GameDifficulty Difficulty;
        public StageDifficulty StageDifficulty; 

        public override string ToString()
        {
            return $"LevelDef(Id: {Id}, Difficulty: {Difficulty}, StageDifficulty: {StageDifficulty})";
        }

        public bool Equals(LevelDef other)
        {
            return Id == other.Id && Difficulty == other.Difficulty && StageDifficulty == other.StageDifficulty;
        }

        public override bool Equals(object obj)
        {
            return obj is LevelDef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Difficulty, StageDifficulty);
        }

        public int CompareTo(LevelDef other)
        {
            // 优先级：GameDifficulty > StageDifficulty > Id
            int difficultyComparison = Difficulty.CompareTo(other.Difficulty);
            if (difficultyComparison != 0)
                return difficultyComparison;

            int stageDifficultyComparison = StageDifficulty.CompareTo(other.StageDifficulty);
            if (stageDifficultyComparison != 0)
                return stageDifficultyComparison;

            return Id.CompareTo(other.Id);
        }

        public static bool operator ==(LevelDef left, LevelDef right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LevelDef left, LevelDef right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(LevelDef left, LevelDef right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(LevelDef left, LevelDef right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(LevelDef left, LevelDef right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(LevelDef left, LevelDef right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}