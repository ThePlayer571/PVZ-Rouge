using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.UI;
using TPL.PVZR.ViewControllers;
using TPL.PVZR.ViewControllers.Others;
using UnityEngine;

namespace TPL.PVZR.Core
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