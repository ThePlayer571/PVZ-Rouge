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
    public interface ISceneTransitionEffectService : IService
    {
        Task PlayTransitionAsync(TransitionEffectType transitionEffectType = TransitionEffectType.Slide);
        Task EndTransition();
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
        }

        public async Task EndTransition()
        {
            if (_transitionState is not ( /*TransitionState.FadingIn or*/ TransitionState.Holding))
            {
                $"在错误的transitionState调用EndTransition：{_transitionState}。".LogError();
                return;
            }

            _transitionState = TransitionState.FadingOut;
            await EndSlideTransitionAsync();
            _transitionState = TransitionState.Idle;
        }

        #endregion

        #region 实现细节

        private TransitionState _transitionState = TransitionState.Idle;
        private TransitionEffectNode _transitionEffectNode;

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
            var panel = UIKit.OpenPanel<UITransitionEffect>();
            panel.transform.SetParent(UIKit.Root.transform.Find("Transition"));
            _transitionEffectNode = panel.GetComponent<TransitionEffectNode>();
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