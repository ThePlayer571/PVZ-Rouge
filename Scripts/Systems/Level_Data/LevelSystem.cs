using System.Collections.Generic;
using Cinemachine;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn;
using TPL.PVZR.CommandEvents.New.Level_Shit;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPL.PVZR.Systems.Level_Data
{
    public interface ILevelSystem : IMainSystem
    {
    }

    public class LevelSystem : AbstractSystem, ILevelSystem
    {
        private ResLoader _ResLoader;
        private IPhaseModel _PhaseModel;
        private IGameModel _GameModel;
        private ILevelModel _LevelModel;
        private ILevelGridModel _LevelGridModel;

        protected override void OnInit()
        {
            _ResLoader = ResLoader.Allocate();
            _PhaseModel = this.GetModel<IPhaseModel>();
            _GameModel = this.GetModel<IGameModel>();
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
                                var tombData = e.Parameters["TombData"] as ITombData;
                                var levelData = GameCreator.CreateLevelData(_GameModel.GameData,
                                    tombData.LevelDefinition.LevelDef);

                                ActionKit.Sequence()
                                    .Callback(() =>
                                    {
                                        _LevelModel.Initialize(levelData);
                                        _GameModel.ActiveTombData = tombData;
                                        SceneManager.LoadScene("LevelScene");
                                    })
                                    .DelayFrame(1) // 等待场景加载
                                    .Callback(() =>
                                    {
                                        var levelSet = levelData.LevelPrefab.Instantiate();
                                        // TODO 地图生成算法
                                    })
                                    .DelayFrame(1) // 等待LevelPrefab加载
                                    .Callback(() =>
                                    {
                                        _LevelGridModel.Initialize(levelData);
                                        //
                                        var DavePrefab = _ResLoader.LoadSync<Player>(Dave_prefab.BundleName,
                                            Dave_prefab.Dave);
                                        //
                                        var VirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
                                        var Player = DavePrefab.Instantiate(levelData.InitialPlayerPos,
                                            Quaternion.identity);
                                        VirtualCamera.Follow = Player.transform;
                                        VirtualCamera.PreviousStateIsValid = false;
                                    })
                                    .DelayFrame(1)
                                    .Callback(() => { this.SendEvent<OnLevelGameObjectPrepared>(); })
                                    // .DelayFrame(1)
                                    .Callback(() => { _PhaseModel.DelayChangePhase(GamePhase.LevelInitialization); })
                                    .Start(GameManager.Instance);
                                break;
                        }

                        break;

                    case GamePhase.LevelInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterLate:
                                _PhaseModel.DelayChangePhase(GamePhase.ChooseSeeds);
                                // 初始化初始植物
                                //todo 在Late阶段是因为AISystem的初始化在Normal，且它遇到初始植物时会出错，等到AiSyatem重构后再解决
                                foreach (InitialPlantConfig config in _LevelModel.LevelData.InitialPlants)
                                {
                                    this.SendEvent<SpawnPlantEvent>(new SpawnPlantEvent
                                    {
                                        Def = config.PlantDef, CellPos = config.SpawnPos, Direction = config.Direction
                                    });
                                }

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
                                var chosenSeedOptions = UIKit.GetPanel<UIChooseSeedPanel>().chosenSeedOptions;
                                // 这里为了确保index对应，没有用List.Add（非常安全）
                                _LevelModel.ChosenSeeds.Resize(chosenSeedOptions.Count);
                                for (int i = 0; i < chosenSeedOptions.Count; i++)
                                {
                                    _LevelModel.ChosenSeeds[i] =
                                        SeedData.Create(i + 1, chosenSeedOptions[i].CardData);
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
                                    .Callback(() => { _PhaseModel.DelayChangePhase(GamePhase.Gameplay); })
                                    .Start(GameManager.Instance);
                                break;
                        }

                        break;
                    case GamePhase.Gameplay:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterLate:
                                // this.SendEvent<OnWaveStart>(new OnWaveStart { Wave = 10 });
                                break;
                        }

                        break;

                    case GamePhase.LevelPassed:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                _PhaseModel.DelayChangePhase(GamePhase.LevelExiting,
                                    new Dictionary<string, object>
                                    {
                                        { "NextPhase", GamePhase.MazeMapInitialization }
                                    });
                                break;
                        }

                        break;
                    case GamePhase.LevelExiting:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterLate:
                                var nextPhase = (GamePhase)e.Parameters["NextPhase"];
                                _PhaseModel.DelayChangePhase(nextPhase);
                                break;
                            case PhaseStage.LeaveNormal:
                                _LevelModel.Reset();
                                _LevelGridModel.Reset();

                                UIKit.ClosePanel<UILevelGameplayPanel>();
                                break;
                        }

                        break;
                }
            });
        }
    }
}