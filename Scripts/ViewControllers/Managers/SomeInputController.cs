using System;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using TPL.PVZR.Tools;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Managers
{
    /// <summary>
    /// 为解耦良好而设立：监听部分Input 
    /// </summary>
    public class SomeInputController : MonoBehaviour, IController
    {
        private PlayerInputControl _inputActions;
        private IHandSystem _HandSystem;
        private ILevelModel _LevelModel;
        private ILevelGridModel _LevelGridModel;
        private IPhaseModel _PhaseModel;

        public void Awake()
        {
            _HandSystem = this.GetSystem<IHandSystem>();
            _LevelModel = this.GetModel<ILevelModel>();
            _LevelGridModel = this.GetModel<ILevelGridModel>();
            _PhaseModel = this.GetModel<IPhaseModel>();
            _inputActions = new PlayerInputControl();

            _inputActions.Level.PlantingLeft.performed += context =>
            {
                if (_PhaseModel.GamePhase != GamePhase.Gameplay) return; // 游戏阶段正确
                if (HandHelper.IsHandOnUI()) return; // 手不在UI上
                if (_HandSystem.HandInfo.Value.HandState != HandState.HaveSeed) return; // 手上持有植物
                var pos = HandHelper.HandCellPos();
                if (!HandHelper.DaveCanReachHand()) return; // 手能够到目标位置
                var id = _HandSystem.HandInfo.Value.PickedSeed.CardData.CardDefinition.Id;
                if (!_LevelGridModel.CanSpawnPlantOn(pos, id)) return; // 

                this.SendCommand<PlantingSeedInHandCommand>(new PlantingSeedInHandCommand(Direction2.Left));
            };

            _inputActions.Level.PlantingRight.performed += context =>
            {
                if (_PhaseModel.GamePhase != GamePhase.Gameplay) return; // 游戏阶段正确
                if (HandHelper.IsHandOnUI()) return; // 手不在UI上
                if (_HandSystem.HandInfo.Value.HandState != HandState.HaveSeed) return; // 手上持有植物
                var pos = HandHelper.HandCellPos();
                if (!HandHelper.DaveCanReachHand()) return; // 手能够到目标位置
                var id = _HandSystem.HandInfo.Value.PickedSeed.CardData.CardDefinition.Id;
                if (!_LevelGridModel.CanSpawnPlantOn(pos, id)) return; // 

                this.SendCommand<PlantingSeedInHandCommand>(new PlantingSeedInHandCommand(Direction2.Right));
            };

            _inputActions.Level.Deselect.performed += (context) =>
            {
                if (_PhaseModel.GamePhase == GamePhase.Gameplay &&
                    _HandSystem.HandInfo.Value.HandState != HandState.Empty)
                {
                    this.SendCommand<DeselectCommand>();
                }
            };

            _inputActions.Level.UseShovel.performed += context =>
            {
                // 异常处理
                if (_PhaseModel.GamePhase != GamePhase.Gameplay) return; // 游戏阶段正确
                if (HandHelper.IsHandOnUI()) return; // 手不在UI上
                if (_HandSystem.HandInfo.Value.HandState != HandState.HaveShovel) return; // 手上持有铲子
                var position = HandHelper.HandCellPos();
                if (!_LevelGridModel.IsValidPos(position)) return; // 手在地图内部
                var targetCell = _LevelGridModel.LevelMatrix[position.x, position.y];
                if (targetCell.CellPlantState != CellPlantState.HavePlant) return;
                //
                this.SendCommand<UseShovelCommand>(new UseShovelCommand(position));
            };

            _inputActions.Level.SelectShovel.performed += context =>
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

            _inputActions.Level.SelectSeed_1.performed += (context) => TrySelectSeedByIndex(1);
            _inputActions.Level.SelectSeed_2.performed += (context) => TrySelectSeedByIndex(2);
            _inputActions.Level.SelectSeed_3.performed += (context) => TrySelectSeedByIndex(3);
            _inputActions.Level.SelectSeed_4.performed += (context) => TrySelectSeedByIndex(4);
            _inputActions.Level.SelectSeed_5.performed += (context) => TrySelectSeedByIndex(5);
            _inputActions.Level.SelectSeed_6.performed += (context) => TrySelectSeedByIndex(6);
            _inputActions.Level.SelectSeed_7.performed += (context) => TrySelectSeedByIndex(7);
            _inputActions.Level.SelectSeed_8.performed += (context) => TrySelectSeedByIndex(8);

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

            // _inputActions的开关
            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                _inputActions.Level.Enable();
                                break;
                        }

                        break;
                    case GamePhase.LevelExiting:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                _inputActions.Level.Disable();
                                break;
                        }

                        break;
                }
            });
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}