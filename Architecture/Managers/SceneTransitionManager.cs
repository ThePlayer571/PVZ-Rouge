using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using QFramework;
using TPL.PVZR.UI;
using DG.Tweening;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Systems;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using UnityEngine.PlayerLoop;

namespace TPL.PVZR.Architecture.Managers
{
    public class SceneTransitionManager : ICanRegisterEvent
    {
        # region 单例

        public static SceneTransitionManager Instance { get; private set; }

        static SceneTransitionManager()
        {
            Instance = new SceneTransitionManager();
            Instance.Init();
        }

        #endregion

        # region 公有

        /// <summary>
        /// 添加Mask的理由
        /// </summary>
        /// <param name="reason"></param>
        public void AddMaskReason(string reason)
        {
            _maskReasons.Add(reason);
        }

        /// <summary>
        /// 减少Mask的理由
        /// </summary>
        /// <param name="reason"></param>
        public void RemoveMaskReason(string reason)
        {
            _maskReasons.Remove(reason);
        }

        public bool isMask => _FSM.CurrentStateId == TransitionState.Mask;
        public bool isUnmask => _FSM.CurrentStateId == TransitionState.Unmask;

        # endregion

        # region 私有

        private enum TransitionState
        {
            Mask,
            Unmask,
            TransToMask,
            TransToUnmask
        }

        private UISceneTransitionPanel _UISceneTransitionPanel;
        private readonly List<string> _maskReasons = new();
        private FSM<TransitionState> _FSM = new();

        private void Init()
        {
            // 初始化的触发
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.PreInitialization)
                {
                    ActionKit.Sequence()
                        .Callback(() =>
                        {
                            // 设置TransUI
                            _UISceneTransitionPanel = UIKit.OpenPanel<UISceneTransitionPanel>();
                            _UISceneTransitionPanel.LogInfo();
                        })
                        .DelayFrame(1) // 等待OpenPanel
                        .Callback(() =>
                        {
                            // Update
                            GameManager.ExecuteOnUpdate(Update);
                            // 状态机
                            _FSM.State(TransitionState.Mask)
                                .OnUpdate(() =>
                                {
                                    if (_maskReasons.Count == 0)
                                    {
                                        _FSM.ChangeState(TransitionState.TransToUnmask);
                                    }
                                });
                            _FSM.State(TransitionState.TransToUnmask)
                                .OnUpdate(() =>
                                {
                                    _UISceneTransitionPanel.Bg.DOFade(0, 0.5f).OnComplete(() =>
                                    {
                                        _FSM.ChangeState(TransitionState.Unmask);
                                    });
                                });
                            _FSM.State(TransitionState.Unmask)
                                .OnUpdate(() =>
                                {
                                    if (_maskReasons.Count > 0)
                                    {
                                        _FSM.ChangeState(TransitionState.TransToMask);
                                    }
                                });
                            _FSM.State(TransitionState.TransToMask)
                                .OnUpdate(() =>
                                {
                                    _UISceneTransitionPanel.Bg.DOFade(1, 0.5f).OnComplete(() =>
                                    {
                                        _FSM.ChangeState(TransitionState.Mask);
                                    });
                                });
                            _FSM.StartState(TransitionState.Unmask);
                        }).Start(GameManager.Instance);
                }
            });
        }

        private void Update()
        {
            _FSM.Update();
        }

        # endregion

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}