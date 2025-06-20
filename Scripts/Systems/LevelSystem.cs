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
        private ILevelModel _LevelModel;

        protected override void OnInit()
        {
            _ResLoader = ResLoader.Allocate();
            _PhaseModel = this.GetModel<IPhaseModel>();
            _LevelModel = this.GetModel<ILevelModel>();

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
                                    .Callback(() =>
                                    {
                                        //
                                        var DavePrefab = _ResLoader.LoadSync<Player>(Dave_prefab.BundleName,
                                            Dave_prefab.Dave);
                                        //
                                        var VirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
                                        var Player = DavePrefab.Instantiate(LevelData.InitialPlayerPos,
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
                                var ChosenSeeds = _LevelModel.ChosenCardData;
                                var chosenSeedOptions = ReferenceHelper.ChooseSeedPanel.chosenSeedOptions;
                                // 因为之后会用index选择Seed，这里也用index的方式设置是最安全的，别改！！！！（不过有可能outOfIndex）
                                ChosenSeeds.Resize(chosenSeedOptions.Count);
                                for (int i = 0; i < chosenSeedOptions.Count; i++)
                                {
                                    ChosenSeeds[i] = chosenSeedOptions[i].CardData;
                                }
                                break;
                        }

                        break;
                    case GamePhase.ReadyToStart:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterEarly:
                                UIKit.OpenPanel<UILevelGameplayPanel>();
                                break;
                        }

                        break;
                }
            });
        }
    }
}