using System.Collections.Generic;
using TPL.PVZR.ViewControllers;
using TPL.PVZR.ViewControllers.Others;
using TPL.PVZR.ViewControllers.Others.LevelScene;
using TPL.PVZR.ViewControllers.Others.UI;
using TPL.PVZR.ViewControllers.UI;

namespace TPL.PVZR.Tools
{
    public static class ReferenceHelper
    {
        public static UIChooseSeedPanel ChooseSeedPanel { get; set; }
        public static UILevelGameplayPanel LevelGameplayPanel { get; set; }
        public static List<SeedController> SeedControllers { get; } = new();
        public static LevelTilemapController LevelTilemap { get; set; }
        public static Player Player { get; set; }
    }
}