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
        MazeMap, // 迷宫地图
        
        GameExiting,

        // 关卡内阶段
        LevelPreInitialization, // 最初的初始化，主要用于构建场景等；之后的初始化可能会调用场景中的东西，所以加了这个阶段
        LevelInitialization, // 初始化关卡
        ChooseCards,
        Gameplay,
        AllEnemyKilled,
        ChooseLoots,
        LevelExiting,

        RandomEvent, // 随机事件（包括商店/问号房间）
        Shop, // 商店（单独列出以便特殊逻辑）
        CombatPreparation, // 战前准备（卡组调整）
        Battle, // 战斗阶段
        BossBattle, // BOSS战（特殊战斗逻辑）
        PostCombatReward, // 战后奖励（卡牌/遗物选择）

        // 游戏结束阶段
        GameOverDefeat, // 失败结局
        GameOverVictory, // 胜利结局
        StatisticsReview, // 数据统计回顾

        // 特殊状态
        Paused, // 游戏暂停
        Cutscene, // 剧情过场动画
        DebugMode // 开发者调试模式
    }
}