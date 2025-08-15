using System;
using QFramework;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Systems.Level_Data;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.UI;

namespace TPL.PVZR.ViewControllers.Managers
{
    public class InputManager : MonoSingleton<InputManager>, IController
    {
        #region Public

        public PlayerInputControl InputActions { get; private set; }

        #endregion

        private IHandSystem _HandSystem;
        private ILevelModel _LevelModel;
        private IGameModel _GameModel;
        private ILevelGridModel _LevelGridModel;
        private IPhaseModel _PhaseModel;

        private void Awake()
        {
            _HandSystem = this.GetSystem<IHandSystem>();
            _LevelModel = this.GetModel<ILevelModel>();
            _GameModel = this.GetModel<IGameModel>();
            _LevelGridModel = this.GetModel<ILevelGridModel>();
            _PhaseModel = this.GetModel<IPhaseModel>();
            InputActions = new PlayerInputControl();

            #region Level

            InputActions.Level.PlantingLeft.performed += _ =>
            {
                if (_PhaseModel.GamePhase != GamePhase.Gameplay) return; // 游戏阶段正确
                if (HandHelper.IsHandOnUI()) return; // 手不在UI上
                if (_HandSystem.HandInfo.Value.HandState != HandState.HaveSeed) return; // 手上持有植物
                var pos = HandHelper.HandCellPos();
                if (!HandHelper.DaveCanReachHand()) return; // 手能够到目标位置
                var def = _HandSystem.HandInfo.Value.PickedSeed.CardData.CardDefinition.PlantDef;
                if (!_LevelGridModel.CanSpawnPlantOn(pos, def) && !_LevelGridModel.CanStackPlantOn(pos, def))
                    return; // 

                this.SendCommand<PlantSeedInHandCommand>(new PlantSeedInHandCommand(Direction2.Left));
            };

            InputActions.Level.PlantingRight.performed += _ =>
            {
                if (_PhaseModel.GamePhase != GamePhase.Gameplay) return; // 游戏阶段正确
                if (HandHelper.IsHandOnUI()) return; // 手不在UI上
                if (_HandSystem.HandInfo.Value.HandState != HandState.HaveSeed) return; // 手上持有植物
                var pos = HandHelper.HandCellPos();
                if (!HandHelper.DaveCanReachHand()) return; // 手能够到目标位置
                var def = _HandSystem.HandInfo.Value.PickedSeed.CardData.CardDefinition.PlantDef;
                if (!_LevelGridModel.CanSpawnPlantOn(pos, def) && !_LevelGridModel.CanStackPlantOn(pos, def))
                    return; // 

                this.SendCommand<PlantSeedInHandCommand>(new PlantSeedInHandCommand(Direction2.Right));
            };

            InputActions.Level.Deselect.performed += _ =>
            {
                if (_PhaseModel.GamePhase == GamePhase.Gameplay &&
                    _HandSystem.HandInfo.Value.HandState != HandState.Empty)
                {
                    this.SendCommand<DeselectCommand>();
                }
            };

            InputActions.Level.UseShovel.performed += _ =>
            {
                // 异常处理
                // $"Conditions: 游戏阶段正确：{_PhaseModel.GamePhase == GamePhase.Gameplay}, 手不在UI上：{!HandHelper.IsHandOnUI()}, 手上持有铲子：{_HandSystem.HandInfo.Value.HandState == HandState.HaveShovel}, 手在地图内部：{_LevelGridModel.IsValidPos(HandHelper.HandCellPos())}, 目标位置有植物：{!_LevelGridModel.LevelMatrix[HandHelper.HandCellPos().x, HandHelper.HandCellPos().y].IsEmpty}"
                //     .LogInfo();
                if (_PhaseModel.GamePhase != GamePhase.Gameplay) return; // 游戏阶段正确
                if (HandHelper.IsHandOnUI()) return; // 手不在UI上
                if (_HandSystem.HandInfo.Value.HandState != HandState.HaveShovel) return; // 手上持有铲子
                var position = HandHelper.HandCellPos();
                if (!_LevelGridModel.IsValidPos(position)) return; // 手在地图内部
                var targetCell = _LevelGridModel.LevelMatrix[position.x, position.y];
                if (targetCell.CellPlantData.IsEmpty()) return;
                if (!HandHelper.DaveCanReach(position)) return;
                //
                this.SendCommand<UseShovelCommand>(new UseShovelCommand(position));
            };

            InputActions.Level.SelectShovel.performed += _ =>
            {
                if (_PhaseModel.GamePhase != GamePhase.Gameplay) return;

                if (_HandSystem.HandInfo.Value.HandState == HandState.Empty)
                {
                    this.SendCommand<SelectShovelCommand>();
                }
                else if (_HandSystem.HandInfo.Value.HandState == HandState.HaveShovel) return;

                else if (_HandSystem.HandInfo.Value.HandState == HandState.HaveSeed)
                {
                    this.SendCommand<DeselectCommand>();
                    this.SendCommand<SelectShovelCommand>();
                }
            };

            InputActions.Level.SelectSeed_1.performed += _ => TrySelectSeedByIndex(1);
            InputActions.Level.SelectSeed_2.performed += _ => TrySelectSeedByIndex(2);
            InputActions.Level.SelectSeed_3.performed += _ => TrySelectSeedByIndex(3);
            InputActions.Level.SelectSeed_4.performed += _ => TrySelectSeedByIndex(4);
            InputActions.Level.SelectSeed_5.performed += _ => TrySelectSeedByIndex(5);
            InputActions.Level.SelectSeed_6.performed += _ => TrySelectSeedByIndex(6);
            InputActions.Level.SelectSeed_7.performed += _ => TrySelectSeedByIndex(7);
            InputActions.Level.SelectSeed_8.performed += _ => TrySelectSeedByIndex(8);

            void TrySelectSeedByIndex(int index)
            {
                if (_PhaseModel.GamePhase != GamePhase.Gameplay) return;
                var targetSeedData = _LevelModel.TryGetSeedDataByIndex(index);
                if (targetSeedData == null) return;
                if (!targetSeedData.ColdTimeTimer.Ready) return;
                if (_LevelModel.SunPoint.Value < targetSeedData.CardData.CardDefinition.SunpointCost) return;
                if (_HandSystem.HandInfo.Value.HandState == HandState.Empty)
                {
                    this.SendCommand<SelectSeedCommand>(new SelectSeedCommand(targetSeedData));
                }
                else if (_HandSystem.HandInfo.Value.HandState == HandState.HaveShovel)
                {
                    this.SendCommand<DeselectCommand>();
                    this.SendCommand<SelectSeedCommand>(new SelectSeedCommand(targetSeedData));
                }
                else if (_HandSystem.HandInfo.Value.HandState == HandState.HaveSeed)
                {
                    if (_HandSystem.HandInfo.Value.PickedSeed.Index == index) return;
                    this.SendCommand<DeselectCommand>();
                    this.SendCommand<SelectSeedCommand>(new SelectSeedCommand(targetSeedData));
                }
            }

            #endregion

            #region GameUI

            var uiStackService = this.GetService<IUIStackService>();
            var gamePhaseChangeService = this.GetService<IGamePhaseChangeService>();

            InputActions.GameUI.Esc.performed += _ =>
            {
                if (!_PhaseModel.IsInRoughPhase(RoughPhase.Process)) return;

                if (_GameModel.IsGamePaused)
                {
                    gamePhaseChangeService.ResumeGame();
                }
                else
                {
                    if (uiStackService.HasPanelInStack())
                    {
                        uiStackService.PopTop();
                    }
                    else
                    {
                        gamePhaseChangeService.PauseGame();
                    }
                }
            };

            #endregion

            #region MainMenu

            InputActions.MainMenu.Esc.performed += _ =>
            {
                if (!_PhaseModel.IsInRoughPhase(RoughPhase.Process)) return;

                if (uiStackService.HasPanelInStack())
                {
                    uiStackService.PopTop();
                }
            };

            #endregion

            // _inputActions的开关
            var phaseService = this.GetService<IPhaseService>();
            phaseService.RegisterCallBack((GamePhase.LevelInitialization, PhaseStage.LeaveLate),
                e => { InputActions.Level.Enable(); });
            phaseService.RegisterCallBack((GamePhase.LevelExiting, PhaseStage.EnterEarly),
                e => { InputActions.Level.Disable(); });
            phaseService.RegisterCallBack((GamePhase.GameInitialization, PhaseStage.EnterNormal),
                e => { InputActions.GameUI.Enable(); });
            phaseService.RegisterCallBack((GamePhase.GameExiting, PhaseStage.EnterNormal),
                e => { InputActions.GameUI.Disable(); });
            this.RegisterEvent<OnGamePaused>(e =>
            {
                if (_PhaseModel.IsInRoughPhase(RoughPhase.Level))
                {
                    InputActions.Level.Disable();
                }
            });
            this.RegisterEvent<OnGameResumed>(e =>
            {
                if (_PhaseModel.IsInRoughPhase(RoughPhase.Level))
                {
                    InputActions.Level.Enable();
                }
            });
            phaseService.RegisterCallBack((GamePhase.MainMenu, PhaseStage.EnterLate),
                _ => { InputActions.MainMenu.Enable(); });
            phaseService.RegisterCallBack((GamePhase.MainMenu, PhaseStage.LeaveEarly),
                _ => { InputActions.MainMenu.Disable(); });
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}