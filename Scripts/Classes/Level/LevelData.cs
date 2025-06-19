using UnityEngine;

namespace TPL.PVZR.Classes.Level
{
    public class LevelData : ILevelData
    {
        public int InitialSunPoint { get; } = 50;
        public Vector2 InitialPlayerPos { get; } = new Vector2(20, 9);
    }
}