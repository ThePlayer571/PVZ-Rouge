using Cinemachine;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.InLevel;
using TPL.PVZR.Architecture.Systems.Interfaces;
using TPL.PVZR.Gameplay.Class.Levels;
using TPL.PVZR.Gameplay.ViewControllers.InLevel;
using TPL.PVZR.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPL.PVZR.Architecture.Systems.PhaseSystems
{
    public interface ILevelSystem : ISystem, IPhaseManageSystem
    {
        void SetCurrentLevel(ILevel level);
    }


    // 构建关卡/管理关卡进程
    public partial class LevelSystem : AbstractSystem, ILevelSystem
    {
        #region ILevelSystem

        public void SetCurrentLevel(ILevel level)
        {
            _levelToBuild = level;
        }

        #endregion

        # region IPhaseManageSystem

        private void RegisterPhaseEvents()
        {
            RegisterLevelInitialization();
            RegisterChooseCards();
            RegisterGameplay();
            RegisterAllEnemyKilled();
            RegisterChooseLoots();
            RegisterDefeat();
            RegisterLevelExiting();
        }

        private void RegisterLevelInitialization()
        {
            this.RegisterEvent<OnEnterPhaseEarlyEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelInitialization)
                {
                    AsyncOperation ao = new();
                    //
                    var _ResLoader = ResLoader.Allocate();
                    // 切换至空场景
                    ActionKit.Sequence()
                        .Callback(() => SceneTransitionManager.Instance.AddMaskReason("LevelInitialization"))
                        .Condition(() => SceneTransitionManager.Instance.isMask)
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
                            var _Dave = _ResLoader.LoadSync<Dave>(Dave_prefab.BundleName, Dave_prefab.Dave)
                                .Instantiate(_levelToBuild.MapConfig.daveInitialPos, Quaternion.identity);
                            // VirtualCamara
                            var _VirtualCamera = Object.FindObjectOfType<CinemachineVirtualCamera>();
                            _VirtualCamera.Follow = _Dave.transform;
                        })
                        .DelayFrame(1) // 确保UIKit成功打开UI
                        .Callback(() => // 配置Framework
                        {
                            // 所有数据
                            _LevelModel.OnLevelInitialization();
                        })
                        .Callback(() => // 结尾
                        {
                            Time.timeScale = 1;
                            SceneTransitionManager.Instance.RemoveMaskReason("LevelInitialization");
                        })
                        .Callback(() => _GamePhaseSystem.ChangePhase(GamePhaseSystem.GamePhase.ChooseCards))
                        .Start(GameManager.Instance);
                }

                ;
            });
        }

        private void RegisterChooseCards()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.ChooseCards)
                {
                    _UILevelChooseCardPanel = UIKit.OpenPanel<UILevelChooseCardPanel>();
                    _UILevelChooseCardPanel.Init();
                }
            });
        }

        private void RegisterGameplay()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.Gameplay)
                {
                    // 为了方便统一管理，这里没有把UILevelChooseCardPanel的操作放到LeaveEvent里
                    // 反正两个阶段也是百分百连着的，不会出问题
                    ActionKit.Sequence()
                        .Callback(() =>
                        {
                            // 将Card转移成为Seed
                            _UILevelPanel = UIKit.OpenPanel<UILevelPanel>();
                            _UILevelPanel.HideInstant();
                            _UILevelPanel.Init();
                        })
                        .DelayFrame(1)
                        .Callback(() =>
                        {
                            // 隐藏菜单
                            _UILevelChooseCardPanel.Hide();
                        })
                        .Delay(0.3f) // 等待UI移出摄像机
                        .Callback(() =>
                        {
                            // 移除选卡UI
                            UIKit.ClosePanel<UILevelChooseCardPanel>();
                            _UILevelChooseCardPanel = null;
                            // 显示准备安放植物
                            // TODO
                        })
                        .Delay(1f)
                        .Callback(() => { _UILevelPanel.Show(); })
                        .Delay(1f)
                        .Start(GameManager.Instance);
                }
            });
            this.RegisterEvent<OnLeavePhaseEvent>(e =>
            {
                if (e.leaveFromPhase is GamePhaseSystem.GamePhase.Gameplay)
                {
                    UIKit.ClosePanel<UILevelPanel>();
                }
            });
        }

        private void RegisterAllEnemyKilled()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
                {
                    if (e.changeToPhase is GamePhaseSystem.GamePhase.AllEnemyKilled)
                    {
                        ResLoader.Allocate().LoadSync<GameObject>("EndLevelLootChest")
                            .Instantiate(_EntitySystem.lastDeadZombiePosition, Quaternion.identity);
                    }
                }
            );
        }

        private void RegisterChooseLoots()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.ChooseLoots)
                {
                    // 抽取战利品
                    var _UILevelChooseLootPanelData = new UILevelChooseLootPanelData(_LevelModel.LootConfig.value,
                        _LevelModel.LootConfig.LootDataList);
                    // 打开面板
                    UIKit.OpenPanel<UILevelChooseLootPanel>(_UILevelChooseLootPanelData);
                }
            });
            this.RegisterEvent<OnLeavePhaseEvent>(e =>
            {
                if (e.leaveFromPhase is GamePhaseSystem.GamePhase.ChooseLoots)
                {
                    UIKit.ClosePanel<UILevelChooseLootPanel>();
                }
            });
        }

        private void RegisterDefeat()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.Defeat)
                {
                    UIKit.OpenPanel<UILevelDefeatPanel>();
                }
            });
        }

        private void RegisterLevelExiting()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelExiting)
                {
                    // 重置数据
                    _LevelModel.OnExiting();
                    // 返回界面（还没做）
                    SceneManager.LoadSceneAsync("GameStartScene");
                }
            });
        }

        # endregion


        # region 私有

        // 引用
        private IGamePhaseSystem _GamePhaseSystem;
        private ILevelModel _LevelModel;
        private IWaveSystem _WaveSystem;
        private IEntitySystem _EntitySystem;
        private IZombieSpawnSystem _ZombieSpawnSystem;

        // 为方便调用而存的变量(史山)
        private UILevelChooseCardPanel _UILevelChooseCardPanel;
        private UILevelPanel _UILevelPanel;
        private ILevel _levelToBuild;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _WaveSystem = this.GetSystem<IWaveSystem>();
            _EntitySystem = this.GetSystem<IEntitySystem>();
            //
            RegisterPhaseEvents();
        }

        #endregion

        # region 公有

        public void TryEndLevel()
            // 调用后，尝试结束游戏，根据当前状态判断是否应该结束
        {
            bool shouldEndLevel = _WaveSystem.currentWave == _LevelModel.WaveConfig.totalWaveCount &&
                                  _EntitySystem.ZombieSet.Count == 0 &&
                                  _ZombieSpawnSystem.activeZombieSpawners.Count == 0;
            if (shouldEndLevel)
            {
            }
        }

        # endregion
    }
}