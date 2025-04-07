using QFramework;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Events.Input;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems;
using TPL.PVZR.Architecture.Systems.InLevel;
using TPL.PVZR.Architecture.Systems.Interfaces;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Gameplay.ViewControllers.UI;
using UnityEngine;

namespace TPL.PVZR.Core
{
    
    public class InputSystem : AbstractSystem
    {
        // Model
        private ILevelModel _LevelModel;
        private IHandSystem _HandSystem;
        private PlayerInputControl inputActions;


        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _HandSystem = this.GetSystem<IHandSystem>();
            //
            inputActions = new PlayerInputControl();
            BindInputActions_1();
            // LevelSystem
            RegisterEvents();
        }
        # region LevelSystem
        
        private void RegisterEvents()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelInitialization)
                {
                    inputActions.Gameplay.Enable();
                }
                else if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelExiting)
                {
                    inputActions.Gameplay.Disable();
                }
            });

        }

        # endregion


        private void BindInputActions_1()
        // 与inputActions直接相关的内容
        {
            // 移动
            inputActions.Gameplay.Jump.performed += (context) =>
            {
                this.SendEvent(new InputJumpEvent());
            };
            GameManager.ExecuteOnUpdate(() =>
            {
                var val = inputActions.Gameplay.Move.ReadValue<float>();
                
                this.SendEvent(new InputMoveEvent {speed = val});
                
            });
            
            // 卡牌选择
            inputActions.Gameplay.Deselect.performed += (context) =>
            {
                this.SendEvent(new InputDeselectEvent());
            };
            inputActions.Gameplay.ChooseSeed_1.performed += (context) =>
            {
                SendSelectForceEventByIndex(1);
            };
            inputActions.Gameplay.ChooseSeed_2.performed += (context) =>
            {
                SendSelectForceEventByIndex(2);
            };
            inputActions.Gameplay.ChooseSeed_3.performed += (context) =>
            {
                SendSelectForceEventByIndex(3);
            };
            inputActions.Gameplay.ChooseSeed_4.performed += (context) =>
            {
                SendSelectForceEventByIndex(4);
            };
            inputActions.Gameplay.ChooseSeed_5.performed += (context) =>
            {
                SendSelectForceEventByIndex(5);
            };
            inputActions.Gameplay.ChooseSeed_6.performed += (context) =>
            {
                SendSelectForceEventByIndex(6);
            };
            inputActions.Gameplay.ChooseSeed_7.performed += (context) =>
            {
                SendSelectForceEventByIndex(7);
            };
            inputActions.Gameplay.ChooseSeed_8.performed += (context) =>
            {
                SendSelectForceEventByIndex(8);
            };
            inputActions.Gameplay.ChooseSeed_9.performed += (context) =>
            {
                SendSelectForceEventByIndex(9);
            };
            // 使用手持物品
            inputActions.Gameplay.PlacePlant.performed += (context) =>
            {
                if (!_HandSystem.isHandOnUI &&
                    _HandSystem.currentHandState == HandSystem.HandState.HavePlant)
                {
                    this.SendEvent(new InputPlacePlantEvent
                    {
                        direction = Mathf.Approximately(context.ReadValue<float>(), 1)
                            ? Direction2.Right
                            : Direction2.Left
                    });
                }
            };
            inputActions.Gameplay.UseShovel.performed += (context) =>
            {
                if (_HandSystem.currentHandState == HandSystem.HandState.HaveShovel)
                {
                    this.SendEvent<InputUseShovelEvent>();
                }
            };
            inputActions.Gameplay.Interact.performed += (context) =>
            {
                this.SendEvent<InputInteractEvent>();
            };
        }
# region Triggers

public void TriggerOnSeedButtonClick(Seed seed)
{
    if (_HandSystem.currentHandState == HandSystem.HandState.Empty)
    {
        this.SendEvent<InputSelectEvent>(new InputSelectEvent { seedIndex = seed.seedIndex });
    }
    else if (_HandSystem.currentHandState is HandSystem.HandState.HavePlant or HandSystem.HandState.HaveShovel)
    {
        this.SendEvent<InputDeselectEvent>();
    }
}

public void TriggerOnShovelButtonClick()
{
    
    if (_HandSystem.currentHandState == HandSystem.HandState.Empty)
    {
        this.SendEvent<InputPickShovelEvent>();
    }
    else if (_HandSystem.currentHandState is  HandSystem.HandState.HavePlant or HandSystem.HandState.HaveShovel )
    {
        this.SendEvent<InputDeselectEvent>();
    }
}
#endregion

        private void BindInputActions_2()
        // 场景内ui触发的Input事件
        {
            // 以前有用的，看情况可以删
        }

        private void UnBindInputActions_2()
        {
            // 以前有用的，看情况可以删
        }
        // 函数
        private void SendSelectForceEventByIndex(int index){
           
            foreach (var seed in ReferenceModel.Get.seeds)
            {
                if (seed.seedIndex == index)
                {
                    this.SendEvent<InputSelectForceEvent>(new InputSelectForceEvent { seedIndex = seed.seedIndex });
                    return;
                }
            }
        }
    }
}