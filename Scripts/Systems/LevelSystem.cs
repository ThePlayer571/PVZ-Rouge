using Cinemachine;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.Level;
using TPL.PVZR.Core;
using TPL.PVZR.Events;
using TPL.PVZR.Helpers.Factory;
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
        private ILevelModel _LevelModel;
        private ILevelGridModel _LevelGridModel;

        protected override void OnInit()
        {
            _ResLoader = ResLoader.Allocate();
            _PhaseModel = this.GetModel<IPhaseModel>();
            _LevelModel = this.GetModel<ILevelModel>();
            _LevelGridModel = this.GetModel<ILevelGridModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelPreInitialization:

                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterEarly:
                                var LevelData = e.Parameters["LevelData"] as LevelData;

                                SceneManager.LoadScene("LevelScene");
                                ActionKit.Sequence()
                                    .DelayFrame(1) // 等待场景加载
                                    .Callback(() => { LevelData.LevelDefinition.LevelPrefab.Instantiate(); })
                                    .DelayFrame(1) // 等待LevelPrefab加载
                                    .Callback(() =>
                                    {
                                        _LevelModel.Initialize(LevelData);
                                        _LevelGridModel.Initialize(LevelData);
                                        //
                                        var DavePrefab = _ResLoader.LoadSync<Player>(Dave_prefab.BundleName,
                                            Dave_prefab.Dave);
                                        //
                                        var VirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
                                        var Player = DavePrefab.Instantiate(LevelData.LevelDefinition.InitialPlayerPos,
                                            Quaternion.identity);
                                        VirtualCamera.Follow = Player.transform;
                                    }).Start(GameManager.Instance);
                                break;
                            case PhaseStage.EnterNormal:
                                // 测试用
                                ActionKit.Sequence()
                                    .DelayFrame(2)
                                    .Callback(() => { _PhaseModel.DelayChangePhase(GamePhase.LevelInitialization); })
                                    .Delay(0.1f)
                                    .Callback(() => { _PhaseModel.ChangePhase(GamePhase.ChooseSeeds); })
                                    .Start(GameManager.Instance);
                                break;
                        }

                        break;
                    case GamePhase.ChooseSeeds:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                ActionKit.Sequence()
                                    .Callback(() => { UIKit.OpenPanel<UIChooseSeedPanel>(); })
                                    .DelayFrame(1)
                                    .Callback(() => { }).Start(GameManager.Instance);
                                break;
                            case PhaseStage.LeaveNormal:
                                // 将选卡数据转录
                                var chosenSeedOptions = ReferenceHelper.ChooseSeedPanel.chosenSeedOptions;
                                // 这里为了确保index对应，没有用List.Add（非常安全）
                                _LevelModel.ChosenSeeds.Resize(chosenSeedOptions.Count);
                                for (int i = 0; i < chosenSeedOptions.Count; i++)
                                {
                                    _LevelModel.ChosenSeeds[i] =
                                        SeedHelper.CreateSeedData(i + 1, chosenSeedOptions[i].CardData);
                                }
                                break;
                        }

                        break;
                    case GamePhase.ReadyToStart:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterEarly:
                                var LevelGameplayPanel = UIKit.OpenPanel<UILevelGameplayPanel>();
                                LevelGameplayPanel.HideUIInstantly();
                                break;
                            case PhaseStage.EnterNormal:
                                ActionKit.Sequence()
                                    .Callback(() => { "准备安置植物！！！".LogInfo(); })
                                    .Delay(2f)
                                    .Callback(() =>
                                    {
                                        ReferenceHelper.LevelGameplayPanel.ShowUI();
                                        _PhaseModel.DelayChangePhase(GamePhase.Gameplay);
                                    })
                                    .Start(GameManager.Instance);
                                break;
                        }

                        break;
                }
            });
        }
    }
}