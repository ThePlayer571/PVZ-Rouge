using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.CommandEvents.Phase;
using UnityEngine;

namespace TPL.PVZR.Models
{
    public interface IPhaseModel : IModel
    {
        GamePhase GamePhase { get; }

        /// <summary>
        /// 切换当前的游戏进程（会自动检查是否合理）
        /// </summary>
        /// <param name="changeToPhase"></param>
        /// <param name="parameters"></param>
        void ChangePhase(GamePhase changeToPhase, Dictionary<string, object> parameters = null);

        /// <summary>
        /// 在当前阶段延迟切换到指定的游戏进程（允许_changingPhase为true时调用，会在当前切换结束后再切换至新的阶段）
        /// </summary>
        /// <param name="changeToPhase"></param>
        /// <param name="parameters"></param>
        void DelayChangePhase(GamePhase changeToPhase, Dictionary<string, object> parameters = null);
    }

    public class PhaseModel : AbstractModel, IPhaseModel
    {
        [SerializeField] private float test;


        #region Public

        public GamePhase GamePhase { get; private set; } = GamePhase.BeforeStart;
        public Dictionary<string, object> lastChangeParameters { get; private set; } = null;

        public void ChangePhase(GamePhase changeToPhase, Dictionary<string, object> parameters = null)
        {
            // 检查错误
            if (!allowedPhaseToFrom.ContainsKey(changeToPhase))
            {
                throw new ArgumentException($"未设置切换规则：{changeToPhase}");
            }

            if (!allowedPhaseToFrom[changeToPhase].Contains(this.GamePhase))
            {
                throw new ArgumentException($"进行了不允许的状态切换：{this.GamePhase}->{changeToPhase}");
            }

            if (_changingPhase) throw new Exception("正在切换状态，不能重复调用ChangePhase");

            _changingPhase = true;
            // 切换状态
            var leaveFromPhase = this.GamePhase;
            this.SendEvent(new OnPhaseChangeEvent
                { GamePhase = leaveFromPhase, PhaseStage = PhaseStage.LeaveEarly, Parameters = lastChangeParameters });
            this.SendEvent(new OnPhaseChangeEvent
                { GamePhase = leaveFromPhase, PhaseStage = PhaseStage.LeaveNormal, Parameters = lastChangeParameters });
            this.SendEvent(new OnPhaseChangeEvent
                { GamePhase = leaveFromPhase, PhaseStage = PhaseStage.LeaveLate, Parameters = lastChangeParameters });
            this.GamePhase = changeToPhase;
            lastChangeParameters = parameters;
            this.SendEvent(new OnPhaseChangeEvent
                { GamePhase = changeToPhase, PhaseStage = PhaseStage.EnterEarly, Parameters = parameters });
            this.SendEvent(new OnPhaseChangeEvent
                { GamePhase = changeToPhase, PhaseStage = PhaseStage.EnterNormal, Parameters = parameters });
            this.SendEvent(new OnPhaseChangeEvent
                { GamePhase = changeToPhase, PhaseStage = PhaseStage.EnterLate, Parameters = parameters });
            // 
            _changingPhase = false;

            // 检查是否有延迟切换
            if (_delayedChangePhase.HasValue)
            {
                var delayed = _delayedChangePhase.Value;
                _delayedChangePhase = null;
                ChangePhase(delayed.changeToPhase, delayed.parameters);
            }
        }

        public void DelayChangePhase(GamePhase changeToPhase, Dictionary<string, object> parameters = null)
        {
            if (_changingPhase)
            {
                // 记录延迟切换
                _delayedChangePhase = (changeToPhase, parameters);
            }
            else
            {
                ChangePhase(changeToPhase, parameters);
            }
        }

        #endregion

        #region Private

        private bool _changingPhase = false;

        // 延迟切换阶段的存储
        private (GamePhase changeToPhase, Dictionary<string, object> parameters)? _delayedChangePhase = null;

        private readonly Dictionary<GamePhase, GamePhase[]> allowedPhaseToFrom = new()
        {
            [GamePhase.PreInitialization] = new[] { GamePhase.BeforeStart },
            [GamePhase.MainMenu] = new[] { GamePhase.PreInitialization, GamePhase.GameExiting },
            [GamePhase.GameInitialization] = new[] { GamePhase.MainMenu },
            [GamePhase.MazeMapInitialization] = new[] { GamePhase.GameInitialization, GamePhase.LevelExiting },
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
            [GamePhase.LevelPassed] = new[] { GamePhase.AllEnemyKilled },
            [GamePhase.LevelExiting] = new[]
                { GamePhase.LevelDefeat, GamePhase.LevelPassed, GamePhase.LevelInterrupted },
            [GamePhase.GameExiting] = new[] { GamePhase.MazeMap },
        };

        #endregion


        protected override void OnInit()
        {
        }
    }
}