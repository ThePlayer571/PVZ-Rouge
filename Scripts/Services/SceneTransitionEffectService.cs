using System.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using QFramework;
using TPL.PVZR.ViewControllers.Others;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Services
{
    public struct OnTransitionEffectHoldingBegin
    {
    }

    public interface ISceneTransitionEffectService : IService
    {
        Task PlayTransitionAsync(TransitionEffectType transitionEffectType = TransitionEffectType.Slide);
        Task EndTransition(bool ignoreDebug = false);

        TransitionState CurrentTransitionState { get; }

        // QF史山
        void InitWith(UITransitionEffect panel);
    }

    public class SceneTransitionEffectService : AbstractService, ISceneTransitionEffectService
    {
        #region 接口

        public async Task PlayTransitionAsync(TransitionEffectType transitionEffectType = TransitionEffectType.Slide)
        {
            if (_transitionState != TransitionState.Idle)
            {
                $"在错误的transitionState调用PlayTransitionAsync：{_transitionState}。".LogError();
                return;
            }

            _transitionState = TransitionState.FadingIn;
            await PlaySlideTransitionAsync();
            _transitionState = TransitionState.Holding;
            this.SendEvent<OnTransitionEffectHoldingBegin>();

            // 记录进入Holding状态的时间
            _holdingStartTime = Time.realtimeSinceStartup;
        }

        public async Task EndTransition(bool ignoreDebug = false)
        {
            if (_transitionState is not ( /*TransitionState.FadingIn or*/ TransitionState.Holding))
            {
                if (!ignoreDebug)
                    $"在错误的transitionState调用EndTransition：{_transitionState}。".LogError();
                return;
            }

            // 如果在Holding状态，需要确保维持至少0.8秒
            if (_transitionState == TransitionState.Holding)
            {
                float elapsedTime = Time.realtimeSinceStartup - _holdingStartTime;
                float remainingTime = HOLDING_MIN_DURATION - elapsedTime;

                if (remainingTime > 0)
                {
                    // 等待剩余时间
                    await Task.Delay(Mathf.RoundToInt(remainingTime * 1000));
                }
            }

            _transitionState = TransitionState.FadingOut;
            await EndSlideTransitionAsync();
            _transitionState = TransitionState.Idle;
        }

        public TransitionState CurrentTransitionState => _transitionState;

        public void InitWith(UITransitionEffect panel)
        {
            panel.transform.SetParent(UIKit.Root.transform.Find("Transition"));
            _transitionEffectNode = panel.GetComponent<TransitionEffectNode>();
        }

        #endregion

        #region 实现细节

        private const float HOLDING_MIN_DURATION = 0.8f; // Holding阶段最短维持时间
        private TransitionState _transitionState = TransitionState.Idle;
        private TransitionEffectNode _transitionEffectNode;
        private float _holdingStartTime; // Holding阶段开始时间

        private async Task PlaySlideTransitionAsync()
        {
            _transitionEffectNode.Show();

            var tween1 = _transitionEffectNode.Slide_Up.DOAnchorPosY(0, 0.4f).SetUpdate(true);
            var tween2 = _transitionEffectNode.Slide_Down.DOAnchorPosY(0, 0.4f).SetUpdate(true);

            await Task.WhenAll(tween1.AsyncWaitForCompletion(), tween2.AsyncWaitForCompletion());
        }

        private async Task EndSlideTransitionAsync()
        {
            var tween1 = _transitionEffectNode.Slide_Up.DOAnchorPosY(Screen.height, 0.6f).SetEase(Ease.InOutQuad)
                .SetUpdate(true);
            var tween2 = _transitionEffectNode.Slide_Down.DOAnchorPosY(-Screen.height, 0.6f).SetEase(Ease.InOutQuad)
                .SetUpdate(true);

            await Task.WhenAll(tween1.AsyncWaitForCompletion(), tween2.AsyncWaitForCompletion());

            _transitionEffectNode.Hide();
        }

        #endregion

        protected override void OnInit()
        {
        }
    }


    public enum TransitionEffectType
    {
        Slide = 0,
        Fade = 1,
    }

    public enum TransitionState
    {
        Idle = 0,
        FadingIn = 1,
        Holding = 2,
        FadingOut = 3,
    }
}