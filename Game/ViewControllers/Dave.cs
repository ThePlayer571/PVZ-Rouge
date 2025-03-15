using UnityEngine;
using QFramework;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

namespace TPL.PVZR
{
    public partial class Dave : ViewController, IController
	{
        public enum JumpState
        {
           NotTwiceJumped, TwiceJumped
        }
        // 框架接口
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
        // Model|System
        private IDaveModel _DaveModel;
        // 引用
        private Rigidbody2D _Rigidbody2D;
        private Collider2D _Collider2D;
        // 初始化
        private void Awake()
        {
            jumpState ??= new FSM<JumpState>();
            _Rigidbody2D = GetComponent<Rigidbody2D>();
            _Collider2D = GetComponent<Collider2D>();
            _DaveModel = this.GetModel<IDaveModel>();
            this.RegisterEvent<InputJumpEvent>(TryJump);
            this.RegisterEvent<InputMoveEvent>(Move);
            //
        }

        private void Start()
        {
            jumpState.State(JumpState.NotTwiceJumped);
            jumpState.State(JumpState.TwiceJumped)
                .OnUpdate(() =>
                {
                    if (isOnGround)
                    {
                        jumpState.ChangeState(JumpState.NotTwiceJumped);
                    }
                })
                ;
            jumpState.StartState(JumpState.NotTwiceJumped);
        }

        // 数据
        private readonly Vector2[] jumpDetectRegion = new Vector2[2];
        // 变量
        private FSM<JumpState> jumpState = new FSM<JumpState>();
        // 属性
        private bool isOnGround => Physics2D.OverlapArea(jumpDetectRegionMin, jumpDetectRegionMax, LayerMask.GetMask("Barrier"));
        private Vector3 jumpDetectRegionMin => _Collider2D.bounds.min + new Vector3(0.1f,-0.1f); // 左下
        private Vector3 jumpDetectRegionMax => _Collider2D.bounds.min + new Vector3(_Collider2D.bounds.extents.x-0.1f,0); // 右上
        // == 逻辑
        private void Update()
        {
            jumpState.Update();
        }
        // 操作
        private void TryJump(InputJumpEvent inputJumpEvent)
        {
            if (isOnGround)
            {
                Jump();
            }
            else
            {
                if (jumpState.CurrentStateId == JumpState.NotTwiceJumped)
                {
                    Jump();
                    jumpState.ChangeState(JumpState.TwiceJumped);
                }
            }
        }

        private void Jump()
        {
            _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, _DaveModel.jumpSpeed);
        }
        private void Move(InputMoveEvent inputMoveEvent)
        {
            _Rigidbody2D.velocity = new Vector2(inputMoveEvent.speed * _DaveModel.moveSpeed, _Rigidbody2D.velocity.y);
        }
        // 函数

    }
}
