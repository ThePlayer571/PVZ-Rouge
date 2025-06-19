using UnityEngine;

namespace TPL.PVZR.Classes.Level
{
    public interface ILevelData
    {
        int InitialSunPoint { get; }
        Vector2 InitialPlayerPos { get; }
    }
}