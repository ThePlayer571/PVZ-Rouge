using UnityEngine;
using QFramework;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TPL.PVZR
{
    
    public class InputSystem : AbstractSystem, IInLevelSystem
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
        }

        private void BindInputActions_1()
        // 与inputActions直接相关的内容
        {
            
            // 移动
            inputActions.Gameplay.Jump.performed += (context) =>
            {
                this.SendEvent<InputJumpEvent>(new());
            };
            GameManager.ExecuteOnFixedUpdate(() =>
            {
                var val = inputActions.Gameplay.Move.ReadValue<float>();
                
                this.SendEvent<InputMoveEvent>(new InputMoveEvent {speed = val});
                
            });
            
            // 卡牌选择
            inputActions.Gameplay.Deselect.performed += (context) =>
            {
                this.SendEvent<InputDeselectEvent>(new());
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
                if (!_HandSystem.handIsOnUI &&
                    _HandSystem.handState == HandSystem.HandState.HavePlant)
                {
                    this.SendEvent<InputPlacePlantEvent>(new()
                        { direction = Mathf.Approximately(context.ReadValue<float>(), 1) ? Direction2.Right : Direction2.Left });
                }
            };
            inputActions.Gameplay.UseShovel.performed += (context) =>
            {
                if (_HandSystem.handState == HandSystem.HandState.HaveShovel)
                {
                    this.SendEvent<InputUseShovelEvent>(new());
                }
            };
        }


        public void TriggerOnSeedButtonClick(Seed seed)
        {
            if (_HandSystem.handState == HandSystem.HandState.Empty)
            {
                this.SendEvent<InputSelectEvent>(new InputSelectEvent { seed = seed });
            }
            else if (_HandSystem.handState is HandSystem.HandState.HavePlant or HandSystem.HandState.HaveShovel)
            {
                this.SendEvent<InputDeselectEvent>();
            }
        }
        private void BindInputActions_2()
        // 场景内ui触发的Input事件
        {
            _LevelModel.shovel.GetComponent<Button>().onClick.AddListener(() =>
            {
                
                if (_HandSystem.handState == HandSystem.HandState.Empty)
                {
                    this.SendEvent<InputPickShovelEvent>(new());
                }
                else if (_HandSystem.handState is  HandSystem.HandState.HavePlant or HandSystem.HandState.HaveShovel )
                {
                    this.SendEvent<InputDeselectEvent>(new());
                }
            });
        }

        private void UnBindInputActions_2()
        {
            _LevelModel.shovel.GetComponent<Button>().onClick.RemoveAllListeners();
            
        }
        public void OnBuildingLevel()
        {
            inputActions.Gameplay.Enable();
        }
        public void OnGameplay()
        {
            BindInputActions_2();
        }

        public void OnEnd()
        {
            inputActions.Gameplay.Disable();
            UnBindInputActions_2();

        }
        // 函数
        private void SendSelectForceEventByIndex(int index)
        {
            foreach (var card in _LevelModel.seeds)
            {
                if (card.seedIndex == index)
                {
                    this.SendEvent<InputSelectForceEvent>(new InputSelectForceEvent { seed = card });
                    return;
                }
            }
        }
    }
}