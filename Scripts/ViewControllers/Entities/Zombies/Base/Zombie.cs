using System;
using QFramework;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Core;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies
{
    public abstract class Zombie : Entity, IZombie
    {
        #region AI / 行为主控

        public IAttackable AttackingTarget { get; set; }

        protected override void Awake()
        {
            base.Awake();
            _FSM = new FSM<ZombieState>();
            _FSM.AddState(ZombieState.DefaultTargeting, new DefaultTargetingState(_FSM, this));
            _FSM.AddState(ZombieState.Attacking, new AttackingState(_FSM, this));

            _FSM.StartState(ZombieState.DefaultTargeting);
        }

        protected override void Update()
        {
            base.Update();
            //
            _FSM.Update();
        }

        private void FixedUpdate()
        {
            // dragForce
            var dragForce = new Vector2(-5 * _Rigidbody2D.velocity.x, 0);
            _Rigidbody2D.AddForce(dragForce);
        }

        private FSM<ZombieState> _FSM;

        #endregion

        #region Effect

        protected EffectGroup effectGroup;


        public void TakeEffect(Effect effect)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region 血量

        protected float health;

        public AttackData TakeAttack(AttackData attackData)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 攻击

        [SerializeField] public ZombieAttackAreaController AttackArea;
        private AttackData BasicAttackData;

        public AttackData CreateCurrentAttackData()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 移动

        [SerializeField] public Direction2 Direction;

        public void MoveForward()
        {
            _Rigidbody2D.AddForce(Direction.ToVector2() * speed);
        }

        #endregion

        protected float speed = 1f;
    }
}