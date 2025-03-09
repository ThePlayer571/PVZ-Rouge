using UnityEngine;
using QFramework;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TPL.PVZR
{
    public class InputSystem : AbstractSystem
    {
        // Model
        IDaveModel _DaveModel;
        IGameModel _GameModel;
        IHandSystem _HandSystem;


        private PlayerInputControl inputActions;
        protected override void OnInit()
        {
            _DaveModel = this.GetModel<IDaveModel>();
            _GameModel = this.GetModel<IGameModel>();
            _HandSystem = this.GetSystem<IHandSystem>();
            //
            inputActions = new();
            //
            this.RegisterEvent<EnterGameSceneInitEvent>(@event => OnEnterGameSceneInit());

        }

        private void OnEnterGameSceneInit()
        {
            inputActions.Gameplay.Enable();
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
            foreach (Card card in _GameModel.cards)
            {
                card.Btn.onClick.AddListener(() =>
                {
                    if (_HandSystem.handState == HandSystem.HandState.Empty)
                    {
                        this.SendEvent<InputSelectEvent>(new() { card = card });
                    }
                    else if (_HandSystem.handState is HandSystem.HandState.HavePlant or HandSystem.HandState.HaveShovel)
                    {
                        this.SendEvent<InputDeselectEvent>();
                    }
                });
            }
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

            _GameModel.shovel.GetComponent<Button>().onClick.AddListener(() =>
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
            // 点击阳光 弃用
            // inputActions.Gameplay.Click.performed += (context) =>
            // {
            //     if (!_HandSystem.handIsOnUI && _HandSystem.handState == HandSystem.HandState.Empty)
            //     {
            //         var hit = Physics2D.Raycast(_HandSystem.handWorldPos, Vector2.zero, Mathf.Infinity,
            //             LayerMask.GetMask("Sun"));
            //         if (hit.collider)
            //         {
            //             this.SendEvent<InputPickSun>(new() { target = hit.collider.gameObject.GetComponent<Sun>() });
            //         }
            //     }
            // };
        }
        // 函数
        private void SendSelectForceEventByIndex(int index)
        {
            foreach (var card in _GameModel.cards)
            {
                if (card.cardIndex == index)
                {
                    this.SendEvent<InputSelectForceEvent>(new InputSelectForceEvent { card = card });
                    break;
                }
            }
        }
    }
}