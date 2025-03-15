﻿using Cinemachine;
using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

namespace TPL.PVZR
{
    public interface ILevelSystem
    {
        public FSM<LevelSystem.LevelState> levelState { get; }
    }


    // 构建关卡/管理关卡进程
    public class LevelSystem : AbstractSystem,ILevelSystem
    {
        # region 通用
        public enum LevelState
        {
            OutOfLevel,BuildingLevel, ChoosingCards, Gameplay, End 
        }

        public FSM<LevelState> levelState { get; private set; }
        private ILevelModel _LevelModel;
        private InputSystem _InputSystem;
        private IHandSystem _HandSystem;
        private IChooseCardSystem _ChooseCardSystem;
        private IWaveSystem _WaveSystem;
        
        // 为方便调用而存的变量
        private UILevelChooseCardPanel _UILevelChooseCardPanel;
        private UIGamePanel _UIGamePanel;
        
        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _InputSystem = this.GetSystem<InputSystem>();
            _HandSystem = this.GetSystem<IHandSystem>();
            _ChooseCardSystem = this.GetSystem<IChooseCardSystem>();
            _WaveSystem = this.GetSystem<IWaveSystem>();
            //
            levelState = new FSM<LevelState>();
            //
            SetUpState();
        }

        private ILevel _levelToBuild;

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
                            _levelToBuild.GetLevelSceneSet().Instantiate();
                            // Dave
                            var _Dave = _ResLoader.LoadSync<Dave>("Dave")
                                .Instantiate(_levelToBuild.daveInitialPos, Quaternion.identity);
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
                            _UIGamePanel = UIKit.OpenPanel<UIGamePanel>();
                            _UIGamePanel.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 200);
                            _UIGamePanel.SetUp();
                            // 初始化InputSystem In Level
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
                            _UIGamePanel.Show();
                            _WaveSystem.OnGameplay();
                        })
                        .Delay(1f)
                        .Start(GameManager.Instance);
                });
            levelState.State(LevelState.OutOfLevel);
            //
            levelState.StartState(LevelState.OutOfLevel);
        }

        public void EnterLevel(ILevel level)
        {
            _levelToBuild = level;
            levelState.ChangeState(LevelState.BuildingLevel);

        }
        #endregion
        
    }
}