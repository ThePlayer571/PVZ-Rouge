using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.Helpers.New.DataReader;
using UnityEngine;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class TombCreator
    {
        public static ITombData CreateTombData(Vector2Int pos, LevelDef levelDef)
        {
            return new TombData(pos, GameConfigReader.GetLevelDefinition(levelDef));
        }
    }
}