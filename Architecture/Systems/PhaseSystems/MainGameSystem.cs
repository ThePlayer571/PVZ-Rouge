using QFramework;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Systems.Interfaces;
using TPL.PVZR.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TPL.PVZR.Architecture.Systems.PhaseSystems
{
    public interface IMainGameSystem : ISystem, IPhaseManageSystem
    {
        
    }
    
    public class MainGameSystem:AbstractSystem,IMainGameSystem
    {
        public enum MainGameState
        {
            BeforeStart,MainMenu,InGame
        }
        # region 私有
        // 引用
        private IGameSystem _GameSystem;
        private IGamePhaseSystem _GamePhaseSystem;
        
        // 初始化
        
        protected override void OnInit()
        {
            _GameSystem = this.GetSystem<IGameSystem>();
            _GamePhaseSystem = this.GetSystem<IGamePhaseSystem>();
            
            RegisterPhaseEvents();
        }

        private void RegisterPhaseEvents()
        {
            RegisterPreInitialization();
            RegisterMainMenu();
        }
        
        // 初始ui
        private void RegisterPreInitialization()
        {
            this.RegisterEvent<OnEnterPhaseEarlyEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.PreInitialization)
                {
                    "call".LogInfo();
                    // 初始化设置
                    ResKit.Init();
                    var _ = ResLoader.Allocate();
                    var gm = _.LoadSync<GameObject>("GameManager").Instantiate();
                    Object.DontDestroyOnLoad(gm);
                    UIKit.Root.SetResolution(1920, 1080, 0);
                    //
                    _GamePhaseSystem.ChangePhase(GamePhaseSystem.GamePhase.MainMenu);
                }
            });
        }
        private void RegisterMainMenu()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.MainMenu)
                {
                    UIKit.OpenPanel<UIGameStartPanel>();
                }
            });
            this.RegisterEvent<OnLeavePhaseEvent>(e =>
            {
                if (e.leaveFromPhase is GamePhaseSystem.GamePhase.MainMenu)
                {
                    ActionKit.Sequence()
                        .Callback(() => { SceneTransitionManager.Instance.AddMaskReason("CloseUIGameStartPanel"); })
                        .Condition(() => SceneTransitionManager.Instance.isMask)
                        .Callback(() =>
                        {
                            UIKit.ClosePanel<UIGameStartPanel>();
                            SceneTransitionManager.Instance.RemoveMaskReason("CloseUIGameStartPanel");
                        })
                        .Start(GameManager.Instance);
                }
            });
        }
        # endregion
    }
}