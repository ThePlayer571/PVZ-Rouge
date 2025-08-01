using System;

namespace TPL.PVZR.Classes.DataClasses.Level
{
    [Serializable]
    public enum LevelId
    {
        NotSet = 0,

        // Dave
        Dave_Lawn = 1,
        Dave_Roof = 2,
        Dave_Fortress = 3,
        Dave_House = 4,
        Dave_FullYard = 5,
        Dave_LawnNight = 6,
        Dave_Bridge = 7,
        Dave_BridgeRainy = 8,
        Dave_RoofRainy = 9,
        Dave_FullYardNight = 10,
        Dave_HouseNight = 11,
        // Dave_
        Dave_Basement, // 僵尸挖地道
        Dave_Garage, // 实验室（瑞克莫迪）
        Dave_Graveyard, // 恶心的出怪点
        // Dave_LawnNight = 2,
        // DaveHouseNight,DaveHouseRainNight, 
        // DaveBackyardRain,
        // DaveRoofNight,DaveRoofRain,
        // PlainLawn,
        // WaterlessLawn, WaterlessLawnNight,
        // ConeheadField,
        // // Dungeon
        // DungeonEntrance, DungeonEntranceNight,
        // MazeChamber, MazeChamberRain,
        // TrapChamber,
        // TreasureChamber,
        // DungeonSewer, DungeonSewerRain,
        // DungeonLibrary,
        // DungeonCorridor,
        // DungeonAtrium, DungeonAtriumRain,
        // DungeonIntersection,
        // DungeonDeadEndCorridor,
        // DungeonLavaRoad,
        // DungeonArena, 
        // DungeonPrison
    }
}