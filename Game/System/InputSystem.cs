using UnityEngine;
using QFramework;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TPL.PVZR
{
    public interface IInLevelSystem
    {
        public void OnEnterLevel();
        public void OnExitLevel();
    }
    public class InputSystem : AbstractSystem, IInLevelSystem
    {
        // Model
        IDaveModel _DaveModel;
        ILevelModel _LevelModel;
        IHandSystem _HandSystem;


        private PlayerInputControl inputActions;
        protected override void OnInit()
        {
            _DaveModel = this.GetModel<IDaveModel>();
            _LevelModel = this.GetModel<ILevelModel>();
            _HandSystem = this.GetSystem<IHandSystem>();
            //
            inputActions = new();
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
            inputActions.Gameplay.ChooseCard_1.performed += (context) =>
            {
                SendSelectForceEventByIndex(1);
            };
            inputActions.Gameplay.ChooseCard_2.performed += (context) =>
            {
                SendSelectForceEventByIndex(2);
            };
            inputActions.Gameplay.ChooseCard_3.performed += (context) =>
            {
                SendSelectForceEventByIndex(3);
            };
            inputActions.Gameplay.ChooseCard_4.performed += (context) =>
            {
                SendSelectForceEventByIndex(4);
            };
            inputActions.Gameplay.ChooseCard_5.performed += (context) =>
            {
                SendSelectForceEventByIndex(5);
            };
            inputActions.Gameplay.ChooseCard_6.performed += (context) =>
            {
                SendSelectForceEventByIndex(6);
            };
            inputActions.Gameplay.ChooseCard_7.performed += (context) =>
            {
                SendSelectForceEventByIndex(7);
            };
            inputActions.Gameplay.ChooseCard_8.performed += (context) =>
            {
                SendSelectForceEventByIndex(8);
            };
            inputActions.Gameplay.ChooseCard_9.performed += (context) =>
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

        private void BindInputActions_2()
        // 场景内ui触发的Input事件
        {
            foreach (Card card in _LevelModel.cards)
            {
                card.Btn.onClick.AddListener(() =>
                {
                    if (_HandSystem.handState == HandSystem.HandState.Empty)
                    {
                        this.SendEvent<InputSelectEvent>(new InputSelectEvent { card = card });
                    }
                    else if (_HandSystem.handState is HandSystem.HandState.HavePlant or HandSystem.HandState.HaveShovel)
                    {
                        this.SendEvent<InputDeselectEvent>();
                    }
                });
            }

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
            foreach (Card card in _LevelModel.cards)
            {
                card.Btn.onClick.RemoveAllListeners();
            }
            _LevelModel.shovel.GetComponent<Button>().onClick.RemoveAllListeners();
            
        }
        public void OnEnterLevel()
        {
            inputActions.Gameplay.Enable();
            BindInputActions_2();
        }

        public void OnExitLevel()
        {
            inputActions.Gameplay.Disable();
            UnBindInputActions_2();

        }
        // 函数
        private void SendSelectForceEventByIndex(int index)
        {
            foreach (var card in _LevelModel.cards)
            {
                if (card.cardIndex == index)
                {
                    this.SendEvent<InputSelectForceEvent>(new InputSelectForceEvent { card = card });
                    return;
                }
            }
        }
    }
}