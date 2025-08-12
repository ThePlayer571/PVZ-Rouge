using QFramework;
using TPL.PVZR.Models;
using UnityEngine;
using QAssetBundle;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace TPL.PVZR.Systems
{
    public interface IMainGameSystem : IMainSystem
    {
    }

    public class MainGameSystem : AbstractSystem, IMainGameSystem
    {
        private AsyncOperationHandle<GameObject> _gameManagerHandle;
        private AsyncOperationHandle<SceneInstance> _mainMenuSceneHandle;

        protected override void OnInit()
        {
            var phaseService = this.GetService<IPhaseService>();

            phaseService.RegisterCallBack((GamePhase.PreInitialization, PhaseStage.EnterEarly), e =>
            {
                ResKit.Init();
                _gameManagerHandle = Addressables.InstantiateAsync("GameManager");
                _gameManagerHandle.Completed += _ => { Object.DontDestroyOnLoad(_gameManagerHandle.Result); };
                UIKit.Root.SetResolution(1920, 1080, 0);

                phaseService.AddAwait(_gameManagerHandle.Task);
                phaseService.ChangePhase(GamePhase.MainMenu);
            });

            phaseService.RegisterCallBack((GamePhase.MainMenu, PhaseStage.EnterNormal), e =>
            {
                _mainMenuSceneHandle = Addressables.LoadSceneAsync("MainMenu");
                UIKit.OpenPanel<UIMainMenuPanel>();
            });
            
            phaseService.RegisterCallBack((GamePhase.MainMenu, PhaseStage.EnterLate), e =>
            {
                var _ = this.GetService<ISceneTransitionEffectService>();
                _.EndTransition(true);
            });
            phaseService.RegisterCallBack((GamePhase.MainMenu, PhaseStage.LeaveNormal),
                e =>
                {
                    UIKit.ClosePanel<UIMainMenuPanel>();
                    ActionKit.Sequence()
                        .Condition(() => SceneManager.GetSceneByName("MainMenu") == null)
                        .Callback(() => { _mainMenuSceneHandle.Release(); }).StartGlobal();
                });
        }

        protected override void OnDeinit()
        {
            _gameManagerHandle.Release();
        }
    }
}