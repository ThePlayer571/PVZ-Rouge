using System;
using QFramework;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Commands.HandCommands;
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

            //TODO TrySelectSeedWithIndex函数改名，这个名字根本不对
            _inputActions.Level.SelectSeed_1.performed += (context) => TrySelectSeedWithIndex(1);
            _inputActions.Level.SelectSeed_2.performed += (context) => TrySelectSeedWithIndex(2);
            _inputActions.Level.SelectSeed_3.performed += (context) => TrySelectSeedWithIndex(3);
            _inputActions.Level.SelectSeed_4.performed += (context) => TrySelectSeedWithIndex(4);
            _inputActions.Level.SelectSeed_5.performed += (context) => TrySelectSeedWithIndex(5);
            _inputActions.Level.SelectSeed_6.performed += (context) => TrySelectSeedWithIndex(6);
            _inputActions.Level.SelectSeed_7.performed += (context) => TrySelectSeedWithIndex(7);
            _inputActions.Level.SelectSeed_8.performed += (context) => TrySelectSeedWithIndex(8);

            void TrySelectSeedWithIndex(int index)
            {
                if (_PhaseModel.GamePhase != GamePhase.Gameplay) return;
                if (!_LevelModel.TryGetSeedControllerWithIndex(index, out var seedController)) return;

                if (_HandSystem.HandState == HandState.Empty)
                {
                    this.SendCommand<SelectSeedCommand>(new SelectSeedCommand(seedController));
                }
                else
                {
                    if (_HandSystem.PickedSeed.Index == index) return;
                    this.SendCommand<DeselectCommand>();
                    this.SendCommand<SelectSeedCommand>(new SelectSeedCommand(seedController));
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