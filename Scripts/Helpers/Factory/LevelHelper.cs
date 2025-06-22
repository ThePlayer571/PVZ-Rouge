using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.Game;
using TPL.PVZR.Classes.Level;

namespace TPL.PVZR.Helpers
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

        private static Dictionary<LevelId, LevelDefinition> _levelDefinitionDict;

        static LevelHelper()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();
            _levelDefinitionDict = new Dictionary<LevelId, LevelDefinition>
            {
                [LevelId.DaveLawn] = resLoader.LoadSync<LevelDefinition>(Leveldefinition.BundleName,
                    Leveldefinition.LevelDefinition_DaveLawn),
            };
        }
    }
}