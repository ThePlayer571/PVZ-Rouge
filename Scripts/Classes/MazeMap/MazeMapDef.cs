using System;

namespace TPL.PVZR.Classes.MazeMap
{
    [Serializable]
    public struct MazeMapDef : IEquatable<MazeMapDef>
    {
        public MazeMapId Id;
        public GameDifficulty Difficulty;

        public override string ToString()
        {
            return $"MazeMapDef(Id: {Id}, Difficulty: {Difficulty})";
        }

        public bool Equals(MazeMapDef other)
        {
            return Id == other.Id && Difficulty == other.Difficulty;
        }

        public override bool Equals(object obj)
        {
            return obj is MazeMapDef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Id, (int)Difficulty);
        }

        public static bool operator ==(MazeMapDef left, MazeMapDef right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MazeMapDef left, MazeMapDef right)
        {
            return !left.Equals(right);
        }
    }
}