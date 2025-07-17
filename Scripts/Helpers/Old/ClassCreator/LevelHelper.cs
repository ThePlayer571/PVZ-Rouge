using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Game.Interfaces;
using TPL.PVZR.Classes.DataClasses.Level;

namespace TPL.PVZR.Helpers.ClassCreator
{
    public static class LevelHelper
    {
        public static ILevelData CreateLevelData(IGameData gameData, LevelId id)
        {
            if (_levelDefinitionDict.TryGetValue(id, out var levelDefinition))
            {
                return new LevelData(gameData, levelDefinition);
            }
            else
            {
                throw new ArgumentException($"未考虑的LevelId：{id}");
            }
        }

    }
}