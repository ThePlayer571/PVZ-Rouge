using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using QFramework;
using TPL.PVZR.Models;

namespace TPL.PVZR.Services
{
    public struct PhaseChangeInfo
    {
        public IReadOnlyDictionary<string, object> Paras;
    }

    public interface IPhaseService : IService
    {
        void ChangePhase(GamePhase phase, params (string, object)[] paras);
        void RegisterCallBack((GamePhase gamePhase, PhaseStage phaseStage) trigger, Action<PhaseChangeInfo> callback);

        /// <summary>
        /// 异步支持，等待task完成后进行下一步操作
        /// </summary>
        /// <param name="task"></param>
        void AddAwait(Task task);
    }

    public class PhaseService : AbstractService, IPhaseService
    {
        // 函数存储
        private Dictionary<(GamePhase, PhaseStage), List<Action<PhaseChangeInfo>>> _callbackDictionary;

        // 防中断设计
        private (GamePhase phase, (string, object)[] paras)? _nextPhase;

        private bool _changingPhase = false;

        //
        private Queue<Task> _awaitQueue;
        private IReadOnlyDictionary<string, object> _lastChangeParameters = null;

        private IPhaseModel _PhaseModel;


        protected override void OnInit()
        {
            _callbackDictionary = new Dictionary<(GamePhase, PhaseStage), List<Action<PhaseChangeInfo>>>();
            _awaitQueue = new Queue<Task>();
            _nextPhase = null;

            _PhaseModel = this.GetModel<IPhaseModel>();
        }

        public bool TryExecute((GamePhase, PhaseStage) key, IReadOnlyDictionary<string, object> para)
        {
            if (_callbackDictionary.TryGetValue(key, out var actions))
            {
                foreach (var action in actions)
                {
                    action.Invoke(new PhaseChangeInfo { Paras = para });
                }

                return true;
            }

            return false;
        }

        public async void ChangePhase(GamePhase phase, params (string, object)[] paras)
        {
            var leaveFrom = _PhaseModel.GamePhase;
            var changeTo = phase;
            var para = new ReadOnlyDictionary<string, object>(paras.ToDictionary(p => p.Item1, p => p.Item2));
            // 检查错误
            if (!allowedPhaseToFrom.ContainsKey(changeTo))
            {
                $"未设置切换规则：{changeTo}".LogWarning();
            }

            if (!allowedPhaseToFrom[changeTo].Contains(leaveFrom))
            {
                $"进行了不允许的状态切换：{leaveFrom}->{changeTo}".LogWarning();
            }

            if (_changingPhase)
            {
                if (_nextPhase.HasValue) throw new Exception("通道堵塞，无法处理新的阶段变更请求");
                _nextPhase = (phase, paras);
                return;
            }


            _changingPhase = true;
            // 离开阶段
            if (TryExecute((leaveFrom, PhaseStage.LeaveEarly), _lastChangeParameters)) await WaitForAllTasks();
            if (TryExecute((leaveFrom, PhaseStage.LeaveNormal), _lastChangeParameters)) await WaitForAllTasks();
            if (TryExecute((leaveFrom, PhaseStage.LeaveLate), _lastChangeParameters)) await WaitForAllTasks();
            // 进入阶段
            _PhaseModel.GamePhase = changeTo;
            if (TryExecute((changeTo, PhaseStage.EnterEarly), para)) await WaitForAllTasks();
            if (TryExecute((changeTo, PhaseStage.EnterNormal), para)) await WaitForAllTasks();
            if (TryExecute((changeTo, PhaseStage.EnterLate), para)) await WaitForAllTasks();
            //
            _changingPhase = false;

            // 
            _lastChangeParameters = para;

            // 处理下一个阶段变更请求
            if (_nextPhase.HasValue)
            {
                var next = _nextPhase.Value;
                _nextPhase = null;
                ChangePhase(next.phase, next.paras);
            }
        }

        public void RegisterCallBack((GamePhase gamePhase, PhaseStage phaseStage) trigger,
            Action<PhaseChangeInfo> callback)
        {
            if (!_callbackDictionary.ContainsKey(trigger))
            {
                _callbackDictionary[trigger] = new List<Action<PhaseChangeInfo>>();
            }

            _callbackDictionary[trigger].Add(callback);
        }

        public void AddAwait(Task task)
        {
            _awaitQueue.Enqueue(task);
        }

        private async Task WaitForAllTasks()
        {
            var tasksToWait = new List<Task>();

            // 将队列中的所有任务移动到列表中
            while (_awaitQueue.Count > 0)
            {
                var task = _awaitQueue.Dequeue();
                if (task != null && !task.IsCompleted)
                {
                    tasksToWait.Add(task);
                }
            }

            // 等待所有任务完成
            if (tasksToWait.Count > 0)
            {
                await Task.WhenAll(tasksToWait);
            }
        }

        private readonly Dictionary<GamePhase, GamePhase[]> allowedPhaseToFrom = new()
        {
            [GamePhase.PreInitialization] = new[] { GamePhase.BeforeStart },
            [GamePhase.MainMenu] = new[] { GamePhase.PreInitialization, GamePhase.GameExiting },
            [GamePhase.GameInitialization] = new[] { GamePhase.MainMenu },
            [GamePhase.MazeMapInitialization] = new[] { GamePhase.GameInitialization, GamePhase.LevelExiting, GamePhase.LevelDefeatPanel },
            [GamePhase.MazeMap] = new[] { GamePhase.MazeMapInitialization },
            [GamePhase.LevelPreInitialization] = new[] { GamePhase.MazeMap },
            [GamePhase.LevelInitialization] = new[] { GamePhase.LevelPreInitialization },
            [GamePhase.ChooseSeeds] = new[] { GamePhase.LevelInitialization },
            [GamePhase.ReadyToStart] = new[] { GamePhase.ChooseSeeds },
            [GamePhase.Gameplay] = new[] { GamePhase.ReadyToStart },
            [GamePhase.AllEnemyKilled] = new[] { GamePhase.Gameplay },
            [GamePhase.LevelInterrupted] =
                new[] { GamePhase.ChooseSeeds, GamePhase.Gameplay, GamePhase.AllEnemyKilled },
            [GamePhase.LevelDefeat] =
                new[] { GamePhase.ChooseSeeds, GamePhase.Gameplay, GamePhase.AllEnemyKilled },
            [GamePhase.LevelDefeatPanel] = new[] { GamePhase.LevelExiting },
            [GamePhase.LevelPassed] = new[] { GamePhase.AllEnemyKilled },
            [GamePhase.LevelExiting] = new[]
                { GamePhase.LevelDefeat, GamePhase.LevelPassed, GamePhase.LevelInterrupted },
            [GamePhase.GameExiting] = new[] { GamePhase.MazeMap, GamePhase.LevelDefeatPanel },
        };
    }

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
        LevelInitialization, // 初始化关卡（非核心数据设置，请在此处设置）
        ChooseSeeds,
        ReadyToStart,
        Gameplay,
        AllEnemyKilled,
        LevelInterrupted, // 关卡中断（仅用于强行退出只主菜单）
        LevelDefeat, // 关卡失败
        LevelDefeatPanel, // 关卡失败面板（显示失败信息等）
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

    public enum PhaseStage
    {
        EnterEarly,
        EnterNormal,
        EnterLate,
        LeaveEarly,
        LeaveNormal,
        LeaveLate
    }
}