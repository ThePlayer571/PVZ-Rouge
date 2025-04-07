using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Managers;

namespace TPL.PVZR.Architecture.Systems.PhaseSystems
{
    public interface IGamePhaseSystem : ISystem
    {
        /// <summary>
        /// 获取当前的游戏进程
        /// </summary>
        GamePhaseSystem.GamePhase currentGamePhase { get; }

        /// <summary>
        /// 获取当前的大致游戏进程
        /// </summary>
        GamePhaseSystem.RoughGamePhase currentRoughGamePhase { get; }

        /// <summary>
        /// 切换当前的游戏进程（会自动检查是否合理）
        /// </summary>
        /// <param name="changeToPhase"></param>
        void ChangePhase(GamePhaseSystem.GamePhase changeToPhase);
    }

    /// <summary>
    /// 管理游戏进程
    /// </summary>
    public class GamePhaseSystem : AbstractSystem, IGamePhaseSystem
    {
        #region Enums

        public enum GamePhase
        {
            // 启动之前(抽象)
            BeforeStart,

            // 启动与初始化阶段
            PreInitialization, // 预初始化（加载核心资源）
            SplashScreen, // 开场动画

            // 菜单导航阶段
            MainMenu, // 主菜单（开始/设置/退出）
            SaveSlotSelection, // 存档位选择
            SettingsMenu, // 设置菜单
            CreditsScreen, // 制作人员名单

            // 核心游戏循环阶段
            GameInitialization, // 初始化游戏（从其他地方进入游戏时调用）
            MazeMap, // 迷宫地图

            // 关卡内阶段
            LevelInitialization,
            ChooseCards,
            Gameplay,
            AllEnemyKilled,
            ChooseLoots,
            Defeat,
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


        public enum LevelState
        {
        }

        public enum RoughGamePhase
        {
            SomeInitialization,
            MainMenu,
            MazeMap,
            Level,
        }

        #endregion

        # region IGamePhaseSystem

        public GamePhase currentGamePhase => _currentGamePhase;

        public RoughGamePhase currentRoughGamePhase
        {
            get
            {
                return _currentGamePhase switch
                {
                    GamePhase.BeforeStart or GamePhase.PreInitialization or GamePhase.GameInitialization
                        or GamePhase.LevelInitialization => RoughGamePhase.SomeInitialization,
                    GamePhase.SplashScreen or GamePhase.MainMenu => RoughGamePhase.MainMenu,
                    GamePhase.MazeMap => RoughGamePhase.MazeMap,
                    GamePhase.LevelInitialization or GamePhase.ChooseCards or GamePhase.Gameplay
                        or GamePhase.AllEnemyKilled or GamePhase.ChooseLoots or GamePhase.Defeat
                        or GamePhase.LevelExiting => RoughGamePhase.Level,
                    _ => throw new NotImplementedException()
                };
            }
        }


        private readonly Dictionary<GamePhase, GamePhase[]> allowedPhaseToFrom = new()
        {
            [GamePhase.PreInitialization] = new GamePhase[] { GamePhase.BeforeStart },
            [GamePhase.MainMenu] = new GamePhase[] { GamePhase.PreInitialization },
            [GamePhase.GameInitialization] = new GamePhase[] { GamePhase.MainMenu },
            [GamePhase.MazeMap] = new GamePhase[] { GamePhase.GameInitialization },
            [GamePhase.LevelInitialization] = new GamePhase[] { GamePhase.MazeMap },
            [GamePhase.ChooseCards] = new GamePhase[] { GamePhase.LevelInitialization },
            [GamePhase.Gameplay] = new GamePhase[] { GamePhase.ChooseCards },
            [GamePhase.AllEnemyKilled] = new GamePhase[] { GamePhase.Gameplay },
            [GamePhase.ChooseLoots] = new GamePhase[] { GamePhase.AllEnemyKilled },
            [GamePhase.Defeat] = new GamePhase[] { GamePhase.Gameplay }
        };

        public void ChangePhase(GamePhase changeToPhase)
        {
            // 检查错误
            // if (!allowedPhaseFromTo.ContainsKey(_currentGamePhase))
            // {
            //     throw new ArgumentException($"未设置切换规则：{_currentGamePhase}");
            // }

            if (!allowedPhaseToFrom[changeToPhase].Contains(_currentGamePhase))
            {
                throw new ArgumentException($"进行了不允许的状态切换：{_currentGamePhase}->{changeToPhase}");
            }

            // 检查通过
            this.SendEvent<OnLeavePhaseEarlyEvent>(new OnLeavePhaseEarlyEvent { leaveFromPhase = _currentGamePhase });
            this.SendEvent<OnLeavePhaseEvent>(new OnLeavePhaseEvent { leaveFromPhase = _currentGamePhase });
            this._currentGamePhase = changeToPhase;
            this.SendEvent<OnEnterPhaseEarlyEvent>(new OnEnterPhaseEarlyEvent { changeToPhase = changeToPhase });
            this.SendEvent<OnEnterPhaseEvent>(new OnEnterPhaseEvent { changeToPhase = changeToPhase });
        }

        #endregion


        private GamePhase _currentGamePhase;


        protected override void OnInit()
        {
            _currentGamePhase = GamePhase.BeforeStart;
            ActionKit.Delay(0.1f, () => { ChangePhase(GamePhase.PreInitialization); }).Start(GameManager.Instance);
        }
    }
}