using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;
namespace TPL.PVZR
{
    public partial class LevelSystem
    {
        
        private void SetUpState()
        {
            // 
            levelState.State(LevelState.BuildingLevel)
                .OnEnter(() =>
                {
                    AsyncOperation ao = new();
                    //
                    var _ResLoader = ResLoader.Allocate();
                    // 切换至空场景
                    ActionKit.Sequence()
                        .Callback(SceneTransitionManager.Hide)
                        .Delay(1f)
                        .Callback(() => // 搭建场景＆基础设置
                        {
                            Time.timeScale = 0;
                            ao = SceneManager.LoadSceneAsync("LevelSceneTemplate");
                            _LevelModel.SetLevel(_levelToBuild);
                        })
                        .Condition(() => ao.isDone) // 成功构建场景
                        .Callback(() => // 配置关卡GameObject
                        {
                            _levelToBuild.MapConfig.GetLevelSceneSet().Instantiate();
                            // Dave
                            var _Dave = _ResLoader.LoadSync<Dave>("Dave")
                                .Instantiate(_levelToBuild.MapConfig.daveInitialPos, Quaternion.identity);
                            // VirtualCamara
                            var _VirtualCamera = Object.FindObjectOfType<CinemachineVirtualCamera>();
                            _VirtualCamera.Follow = _Dave.transform;
                            // UI
                            UIKit.ClosePanel<UIGameStartPanel>();
                        })
                        .DelayFrame(1) // 确保UIKit成功打开UI
                        .Callback(() => // 配置Framework
                        {
                            // 所有数据
                            _LevelModel.OnBuildingLevel();
                            // 系统
                            _InputSystem.OnBuildingLevel();
                            _HandSystem.OnBuildingLevel();
                        })
                        .Callback(() => // 结尾
                        {
                            Time.timeScale = 1;
                            SceneTransitionManager.Display();
                        })
                        .Callback(() => levelState.ChangeState(LevelState.ChoosingCards))
                        .Start(GameManager.Instance);
                });
            //
            levelState.State(LevelState.ChoosingCards)
                .OnEnter(() =>
                {
                    _ChooseCardSystem.OnChoosingCard();
                    _UILevelChooseCardPanel = UIKit.OpenPanel<UILevelChooseCardPanel>();
                    _UILevelChooseCardPanel.Init();
                });
            //
            levelState.State(LevelState.Gameplay)
                .OnEnter(() =>
                {
                    ActionKit.Sequence()
                        .Callback(() => 
                        {
                            // 将Card转移成为Seed
                            _UILevelPanel = UIKit.OpenPanel<UILevelPanel>();
                            _UILevelPanel.HideQuick();
                            _UILevelPanel.SetUp();
                            // 初始化 InputSystem In Level
                            _LevelModel.OnGameplay();
                            _InputSystem.OnGameplay();
                            _HandSystem.OnGameplay();
                            
                            // 隐藏菜单
                            _UILevelChooseCardPanel.Hide();
                        })
                        .Delay(0.3f) // 等待UI移出摄像机
                        .Callback(() =>
                        {
                            // 移除选卡UI
                            _UILevelChooseCardPanel = null;
                            UIKit.ClosePanel<UILevelChooseCardPanel>();
                            // 显示准备安放植物
                            "显示准备安放植物界面".LogInfo();
                        })
                        .Delay(1f)
                        .Callback(() =>
                        {
                            _UILevelPanel.Show();
                            _WaveSystem.OnGameplay();
                        })
                        .Delay(1f)
                        .Start(GameManager.Instance);
                })
                .OnUpdate(() =>
                {
                    
                });
            //
            levelState.State(LevelState.End)
                .OnEnter(() =>
                {
                    UIKit.ClosePanel<UILevelPanel>();
                    ResLoader.Allocate().LoadSync<GameObject>("EndLevelLootChest").Instantiate(_EntitySystem.lastDeadZombiePosition,Quaternion.identity);
                });
            //
            levelState.State(LevelState.ChooseLoots)
                .OnEnter(() =>
                {
                    // 抽取战利品
                    var _UILevelChooseLootPanelData = new UILevelChooseLootPanelData(_LevelModel.LootConfig.value,
                        _LevelModel.LootConfig.LootDataList);
                    // 打开面板
                    UIKit.OpenPanel<UILevelChooseLootPanel>(_UILevelChooseLootPanelData);
                });
            
            levelState.State(LevelState.Exiting)
                .OnEnter(() =>
                {
                    UIKit.ClosePanel<UILevelChooseLootPanel>();
                    SceneManager.LoadSceneAsync("GameStartScene");
                });
            levelState.State(LevelState.OutOfLevel);
            //
            levelState.StartState(LevelState.OutOfLevel);
        }
    }
}