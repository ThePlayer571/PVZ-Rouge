using System;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Commands.HandCommands;
using TPL.PVZR.Core;
using TPL.PVZR.Events;
using TPL.PVZR.Events.HandEvents;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
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
        private IPhaseModel _PhaseModel;

        public void Awake()
        {
            _HandSystem = this.GetSystem<IHandSystem>();
            _LevelModel = this.GetModel<ILevelModel>();
            _PhaseModel = this.GetModel<IPhaseModel>();
            _inputActions = new PlayerInputControl();

            _inputActions.Level.Deselect.performed += (context) =>
            {
                if (_PhaseModel.GamePhase == GamePhase.Gameplay && _HandSystem.HandState != HandState.Empty)
                {
                    this.SendCommand<DeselectCommand>();
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

                if (_HandSystem.HandState == HandState.Empty)
                {
                    this.SendCommand<SelectSeedCommand>(new SelectSeedCommand(targetSeedData));
                }
                else
                {
                    if (_HandSystem.PickedSeed.Index == index) return;
                    this.SendCommand<DeselectCommand>();
                    this.SendCommand<SelectSeedCommand>(new SelectSeedCommand(targetSeedData));
                }
            }

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