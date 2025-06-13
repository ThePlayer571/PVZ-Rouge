using QFramework;
using TPL.PVZR.Events;
using TPL.PVZR.Models;
using UnityEngine;
using QAssetBundle;
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
            "call MainGameSystem.OnInit".LogInfo();
            this.RegisterEvent<OnEnterPhaseEarlyEvent>(e =>
            {
                if (e.changeToPhase == GamePhase.PreInitialization)
                {
                    "call MainGameSystem.OnInit.OnEnterPhaseEarlyEvent".LogInfo();
                    // 初始化设置
                    ResKit.Init();
                    var resLoader = ResLoader.Allocate();
                    var gm = resLoader.LoadSync<GameObject>(Gamemanager_prefab.BundleName, Gamemanager_prefab.GameManager).Instantiate();
                    Object.DontDestroyOnLoad(gm);
                    UIKit.Root.SetResolution(1920, 1080, 0);

                    this.GetModel<IPhaseModel>().DelayChangePhase(GamePhase.MainMenu);
                }
            });

            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase == GamePhase.MainMenu)
                {
                    UIKit.OpenPanel<UIGameStartPanel>();
                }
            });
        }
    }
}