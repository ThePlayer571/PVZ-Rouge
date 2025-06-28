using QFramework;
using TPL.PVZR.Models;
using UnityEngine;
using QAssetBundle;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.ViewControllers.UI;

namespace TPL.PVZR.Systems
{
    public interface IMainGameSystem : ISystem
    {
    }

    public class MainGameSystem : AbstractSystem, IMainGameSystem
    {
        protected override void OnInit()
        {
            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.PreInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterEarly:
                                // 初始化设置
                                ResKit.Init();
                                var resLoader = ResLoader.Allocate();
                                var gm = resLoader.LoadSync<GameObject>(Gamemanager_prefab.BundleName,
                                    Gamemanager_prefab.GameManager).Instantiate();
                                Object.DontDestroyOnLoad(gm);
                                UIKit.Root.SetResolution(1920, 1080, 0);

                                this.GetModel<IPhaseModel>().DelayChangePhase(GamePhase.MainMenu);
                                break;
                        }

                        break;
                    case GamePhase.MainMenu:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                UIKit.OpenPanel<UIGameStartPanel>();
                                break;
                            case PhaseStage.LeaveLate:
                                UIKit.ClosePanel<UIGameStartPanel>();
                                break;
                        }
                        break;
                }
            });
        }
    }
}