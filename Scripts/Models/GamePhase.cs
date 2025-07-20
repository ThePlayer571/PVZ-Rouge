using System;

namespace TPL.PVZR.Models
{
    public enum GamePhase
    {
        // 启动之前（初始的默认状态）
        BeforeStart,

        // 启动与初始化阶段
        PreInitialization, // 预初始化（加载核心资源）
        SplashScreen, // 开场动画

        // 菜单导航阶段
        MainMenu, // 主菜单（开始/设置/退出）

        // 核心游戏循环阶段
        GameInitialization, // 初始化游戏（从其他地方进入游戏时调用）
        MazeMapInitialization, // 初始化迷宫地图（生成/加载）
        MazeMap, // 迷宫地图

        // 关卡内阶段
        LevelPreInitialization, // 最初的初始化，主要用于构建场景等；之后的初始化可能会调用场景中的东西，所以加了这个阶段
        LevelInitialization, // 初始化关卡
        ChooseSeeds,
        ReadyToStart,
        Gameplay,
        AllEnemyKilled,
        LevelInterrupted, // 关卡中断（仅用于强行退出只主菜单）
        LevelDefeat, // 关卡失败
        LevelPassed, // 关卡通关
        LevelExiting, // 离开关卡（仅用于清除关卡的数据，不涉及GameData）

        // 游戏结束阶段
        GameExiting,


        GameOverDefeat, // 失败结局
        GameOverVictory, // 胜利结局
        StatisticsReview, // 数据统计回顾

        // 特殊状态
        Paused, // 游戏暂停
        Cutscene, // 剧情过场动画
        DebugMode // 开发者调试模式
    }

    public enum RoughPhase
    {
        Game,
        Level,
        Loading
    }

    public static class GamePhaseExtensions
    {
        public static bool IsInRoughPhase(this GamePhase gamePhase, RoughPhase roughPhase)
        {
            return roughPhase switch
            {
                RoughPhase.Game => gamePhase is GamePhase.GameInitialization or GamePhase.MazeMapInitialization
                    or GamePhase.MazeMap or GamePhase.LevelPreInitialization or GamePhase.LevelInitialization
                    or GamePhase.ChooseSeeds or GamePhase.ReadyToStart or GamePhase.Gameplay or GamePhase.AllEnemyKilled
                    or GamePhase.LevelExiting or GamePhase.LevelInterrupted or GamePhase.LevelPassed,
                RoughPhase.Level => gamePhase is GamePhase.LevelPreInitialization or GamePhase.LevelInitialization
                    or GamePhase.ChooseSeeds or GamePhase.ReadyToStart or GamePhase.Gameplay or GamePhase.AllEnemyKilled
                    or GamePhase.LevelExiting or GamePhase.LevelInterrupted or GamePhase.LevelPassed,
                RoughPhase.Loading => gamePhase is GamePhase.GameInitialization or GamePhase.MazeMapInitialization
                    or GamePhase.LevelPreInitialization or GamePhase.LevelInitialization or GamePhase.LevelInterrupted
                    or GamePhase.LevelPassed or GamePhase.LevelExiting or GamePhase.GameExiting,
                _ => throw new ArgumentOutOfRangeException(nameof(roughPhase), roughPhase, null)
            };
        }
    }
}