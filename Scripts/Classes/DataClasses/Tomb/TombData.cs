using System;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Tools.Save;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Tomb
{
    public interface ITombData : ISavable<TombSaveData>
    {
        int Stage { get; }
        Vector2Int Position { get; }
        LevelDefinition LevelDefinition { get; }
    }

    public class TombData : ITombData, IEquatable<TombData>
    {
        public TombData(Vector2Int pos, LevelDefinition levelDefinition, int stage)
        {
            Stage = stage;
            Position = pos;
            LevelDefinition = levelDefinition;
        }

        public int Stage { get; }
        public Vector2Int Position { get; }

        // 经过验证：推荐存储LevelDefinition而不是LevelData
        public LevelDefinition LevelDefinition { get; }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public TombData(TombSaveData saveData)
        {
            Stage = saveData.stage;
            Position = new Vector2Int(saveData.positionX, saveData.positionY);
            LevelDefinition = GameConfigReader.GetLevelDefinition(saveData.levelDef);
        }
        public TombSaveData ToSaveData()
        {
            return new TombSaveData
            {
                stage = Stage,
                positionX = Position.x,
                positionY = Position.y,
                levelDef = LevelDefinition.LevelDef
            };
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