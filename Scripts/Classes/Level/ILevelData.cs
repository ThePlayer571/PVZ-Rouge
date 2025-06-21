using TPL.PVZR.Classes.GameStuff;
using UnityEngine;

namespace TPL.PVZR.Classes.Level
{
    public interface ILevelData
    {
        int InitialSunPoint { get; }

        GlobalEntityData GlobalEntityData { get; }

        // Definition
        LevelDefinition LevelDefinition { get; }
    }
}