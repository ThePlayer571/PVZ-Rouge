using Cinemachine;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.Level;
using TPL.PVZR.Core;
using TPL.PVZR.Events;
using TPL.PVZR.Models;
using TPL.PVZR.UI;
using TPL.PVZR.ViewControllers;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPL.PVZR.Systems
{
    public interface ILevelSystem : ISystem
    {
    }

    public class LevelSystem : AbstractSystem, ILevelSystem
    {
        private ResLoader _ResLoader;
        private IPhaseModel _PhaseModel;

        protected override void OnInit()
        {
            _ResLoader = ResLoader.Allocate();
            _PhaseModel = this.GetModel<IPhaseModel>();

            this.RegisterEvent<OnEnterPhaseEarlyEvent>(e =>
            {
                switch (e.changeToPhase)
                {
                    case GamePhase.LevelPreInitialization:
                        var LevelData = e.parameters["LevelData"] as LevelData;

                        SceneManager.LoadScene("LevelScene");
                        ActionKit.Sequence()
                            .DelayFrame(1)
                            .Callback(() =>
                            {
                                //
                                var DavePrefab = _ResLoader.LoadSync<Player>(Dave_prefab.BundleName, Dave_prefab.Dave);
                                //
                                var VirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
                                var Player = DavePrefab.Instantiate(LevelData.InitialPlayerPos, Quaternion.identity);
                                VirtualCamera.Follow = Player.transform;
                            }).Start(GameManager.Instance);
                        break;
                }
            });
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                switch (e.changeToPhase)
                {
                    case GamePhase.ChooseSeeds:
                        ActionKit.Sequence()
                            .Callback(() => { UIKit.OpenPanel<UIChooseSeedPanel>(); })
                            .DelayFrame(1)
                            .Callback(() =>
                            {
                                var chooseCardPanel = ReferenceHelper.ChooseSeedPanel;
                                var go = CardHelper.CreateSeedOptionController(
                                    CardHelper.GetCardData(PlantId.PeaShooter));
                                go.transform.SetParent(chooseCardPanel.InventorySeeds);
                            }).Start(GameManager.Instance);
                        break;
                    case GamePhase.LevelPreInitialization:
                        ActionKit.Sequence()
                            .Callback(() => { _PhaseModel.DelayChangePhase(GamePhase.LevelInitialization); })
                            .Delay(0.1f)
                            .Callback(() => { _PhaseModel.ChangePhase(GamePhase.ChooseSeeds); })
                            .Start(GameManager.Instance);
                        break;
                }
            });
        }
    }
}