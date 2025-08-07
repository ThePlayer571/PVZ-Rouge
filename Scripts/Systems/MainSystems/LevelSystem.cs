using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn;
using TPL.PVZR.CommandEvents.New.Level_Shit;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
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
        private IPlantService _PlantService;
        private IZombieService _ZombieService;

        private AsyncOperationHandle<SceneInstance> _levelSceneHandle;
        private AsyncOperationHandle<GameObject> _daveHandle;
        private AsyncOperationHandle<GameObject> _levelSetHandle;
        private AsyncOperationHandle<SceneInstance> _levelDefeatSceneHandle;

        protected override void OnInit()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
            _GameModel = this.GetModel<IGameModel>();
            _LevelModel = this.GetModel<ILevelModel>();
            _LevelGridModel = this.GetModel<ILevelGridModel>();
            _PlantService = this.GetService<IPlantService>();
            _ZombieService = this.GetService<IZombieService>();

            var phaseService = this.GetService<IPhaseService>();
            var gamePhaseChangeService = this.GetService<IGamePhaseChangeService>();

            #region 初始化阶段

            phaseService.RegisterCallBack((GamePhase.LevelPreInitialization, PhaseStage.EnterEarly), e =>
            {
                // 数据设置
                var tombData = e.Paras["TombData"] as ITombData;
                var levelData = GameCreator.CreateLevelData(_GameModel.GameData, tombData.LevelDefinition.LevelDef);
                _LevelModel.Initialize(levelData);
                _GameModel.ActiveTombData = tombData;
                // 场景加载
                _levelSceneHandle = Addressables.LoadSceneAsync("LevelScene");
                _daveHandle = Addressables.LoadAssetAsync<GameObject>("Dave");
                _levelSetHandle = Addressables.LoadAssetAsync<GameObject>(levelData.LevelPrefab);

                phaseService.AddAwait(_levelSceneHandle.Task);
                phaseService.AddAwait(_daveHandle.Task);
                phaseService.AddAwait(_levelSetHandle.Task);
            });

            phaseService.RegisterCallBack((GamePhase.LevelPreInitialization, PhaseStage.EnterNormal), e =>
            {
                var levelSet = _levelSetHandle.Result.Instantiate();
                // 等待场景物体Start/Awake执行
                phaseService.AddAwait(ActionTask.WaitForFrame(1));
            });

            phaseService.RegisterCallBack((GamePhase.LevelPreInitialization, PhaseStage.EnterLate), e =>
            {
                // LevelPrefab加载 TODO 地图生成算法
                // 时序：需要根据地图Tilemap生成数据
                _LevelGridModel.Initialize(_LevelModel.LevelData);
                //
                var VirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
                var Player =
                    _daveHandle.Result.Instantiate(_LevelModel.LevelData.InitialPlayerPos, Quaternion.identity);
                VirtualCamera.Follow = Player.transform;
                VirtualCamera.PreviousStateIsValid = false;
                //
                this.SendEvent<OnLevelGameObjectPrepared>();
                phaseService.ChangePhase(GamePhase.LevelInitialization);
            });

            phaseService.RegisterCallBack((GamePhase.LevelInitialization, PhaseStage.EnterNormal), e =>
            {
                // 初始化初始植物
                foreach (InitialPlantConfig config in _LevelModel.LevelData.InitialPlants)
                {
                    _PlantService.SpawnPlant(config.PlantDef, config.SpawnPos, config.Direction);
                }

                //
                phaseService.ChangePhase(GamePhase.ChooseSeeds);
            });

            #endregion

            #region 选卡阶段

            phaseService.RegisterCallBack((GamePhase.ChooseSeeds, PhaseStage.EnterNormal),
                e => { UIKit.OpenPanel<UIChooseSeedPanel>(); });

            phaseService.RegisterCallBack((GamePhase.ChooseSeeds, PhaseStage.LeaveNormal), e =>
            {
                // 将选卡数据转录
                var chosenSeedOptions = UIKit.GetPanel<UIChooseSeedPanel>().chosenSeedOptions;
                // 这里为了确保index对应，没有用List.Add（非常安全）
                _LevelModel.ChosenSeeds.Resize(chosenSeedOptions.Count);
                for (int i = 0; i < chosenSeedOptions.Count; i++)
                {
                    _LevelModel.ChosenSeeds[i] =
                        SeedData.Create(i + 1, chosenSeedOptions[i].CardData);
                }

                //
                ActionKit.Sequence()
                    .Delay(1f)
                    .Callback(UIKit.ClosePanel<UIChooseSeedPanel>)
                    .StartGlobal();
            });

            #endregion

            #region 核心阶段

            phaseService.RegisterCallBack((GamePhase.ReadyToStart, PhaseStage.EnterNormal), e =>
            {
                // 加载UI面板
                var panel = UIKit.OpenPanel<UILevelGameplayPanel>();
                panel.HideUIInstantly();
                //
                NoticeHelper.Notice("准备安放植物！！");
                //
                ActionKit.Sequence()
                    .Delay(1)
                    .Callback(() => phaseService.ChangePhase(GamePhase.Gameplay))
                    .StartGlobal();
            });
            phaseService.RegisterCallBack((GamePhase.Gameplay, PhaseStage.EnterNormal), e =>
            {
                var panel = UIKit.GetPanel<UILevelGameplayPanel>();
                panel._allowChangeUI = true;
                panel.ShowUI();
            });

            phaseService.RegisterCallBack((GamePhase.AllEnemyKilled, PhaseStage.EnterNormal), e =>
            {
                var panel = UIKit.GetPanel<UILevelGameplayPanel>();
                panel._allowChangeUI = false;
                panel.HideUISlotPanel();
            });

            phaseService.RegisterCallBack((GamePhase.AllEnemyKilled, PhaseStage.LeaveNormal), e =>
            {
                //
                UIKit.ClosePanel<UILevelGameplayPanel>();
            });

            #endregion

            #region 结算阶段

            phaseService.RegisterCallBack((GamePhase.LevelPassed, PhaseStage.EnterNormal), e =>
            {
                //
                phaseService.ChangePhase(GamePhase.LevelExiting, ("NextPhase", GamePhase.MazeMapInitialization));
            });

            phaseService.RegisterCallBack((GamePhase.LevelDefeat, PhaseStage.EnterNormal), e =>
            {
                //
                phaseService.ChangePhase(GamePhase.LevelExiting, ("NextPhase", GamePhase.LevelDefeatPanel));
            });

            #endregion

            #region 退出阶段

            phaseService.RegisterCallBack((GamePhase.LevelExiting, PhaseStage.EnterNormal), e =>
            {
                // 卸载场景内容（部分，仅卸载了会报错的）
                _ZombieService.RemoveAllZombies();
                //
                var nextPhase = (GamePhase)e.Paras["NextPhase"];
                phaseService.ChangePhase(nextPhase);
            });

            phaseService.RegisterCallBack((GamePhase.LevelExiting, PhaseStage.LeaveNormal), e =>
            {
                _LevelModel.Reset();
                _LevelGridModel.Reset();

                ActionKit.Sequence()
                    .Condition(() => SceneManager.GetSceneByName("LevelScene") == null)
                    .Callback(() =>
                    {
                        _levelSceneHandle.Release();
                        _daveHandle.Release();
                        _levelSetHandle.Release();
                    })
                    .StartGlobal();

                UIKit.ClosePanel<UILevelGameplayPanel>();
            });

            phaseService.RegisterCallBack((GamePhase.LevelDefeatPanel, PhaseStage.EnterNormal), e =>
            {
                _levelDefeatSceneHandle = Addressables.LoadSceneAsync("LevelDefeatScene");
                UIKit.OpenPanel<UILevelDefeatPanel>();
                phaseService.AddAwait(_levelDefeatSceneHandle.Task);
            });

            phaseService.RegisterCallBack((GamePhase.LevelDefeatPanel, PhaseStage.LeaveNormal), e =>
            {
                UIKit.ClosePanel<UILevelDefeatPanel>();
                ActionKit.Sequence()
                    .Condition(() => SceneManager.GetSceneByName("LevelDefeatScene") == null)
                    .Callback(() => { _levelDefeatSceneHandle.Release(); })
                    .StartGlobal();
            });

            #endregion
        }
    }
}