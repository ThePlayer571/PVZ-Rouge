using System;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Plants.Base
{
    public interface IPlant : IEntity, IAttackable
    {
    }

    public enum PlantState
    {
        Idle,
        Dead
    }

    public abstract class Plant : Entity, IPlant
    {
        #region Behavior

        #region 变量
        // 可配置数据
        [SerializeField] protected float _initialHealthPoint_ = 100;
        
/// <summary>
/// 植物的血量像樱桃炸弹一样（不会被常规方式杀死）
/// </summary>
        [SerializeField] protected bool _cherryBoomHealth_ = false;
        // 变量
        protected Direction2 direction;

        protected BindableProperty<float> healthPoint;
        protected FSM<PlantState> behaviorState = new();

        #endregion
        
        #region 一层具象(可调用方法)

        public void TakeDamage(Attack attack)
        { 
            // 参数检查
            if (attack is null) throw new ArgumentNullException();
            if (_cherryBoomHealth_) return;
            this.healthPoint.Value -= attack.damageValue;
        }

        public override void Kill()
        {
            DOTween.Kill(transform);
            healthPoint.Value = 0;
        }

        protected virtual void Dead()
        {
            _LevelModel.CellGrid[gridPos2.x, gridPos2.y].SetState( Cell.CellState.Empty);
            _LevelModel.CellGrid[gridPos2.x, gridPos2.y].SetPlant(null);
            gameObject.DestroySelf();
        }

        #endregion

        # region 逻辑

        protected virtual void DefaultAI()
        {
            // 必须是空的，因为有些植物override了，而且无法跳过PeaShooterBase直接执行Plant的DefaultAI
        }

        protected virtual void SetUpState()
        {
            behaviorState.State(PlantState.Idle)
                .OnUpdate(() => { DefaultAI(); });
            behaviorState.State(PlantState.Dead)
                .OnEnter(() => { Dead(); });
            behaviorState.StartState(PlantState.Idle);
        }

        # endregion
        
        #endregion
        
        private void Update()
        {
            behaviorState.Update();
        }

        // 初始化
        public virtual void Initialize(Direction2 direction)
        {
            this.direction = direction;
            gameObject.LocalScaleX(direction == Direction2.Right ? 1 : -1);
        }

        // 初始化
        protected override void OnAwakeBase()
        {
            healthPoint = new BindableProperty<float>(_initialHealthPoint_);
            healthPoint.Register((val) =>
            {
                if (val <= 0)
                {
                    behaviorState.ChangeState(PlantState.Dead);
                }
            });
            SetUpState();
        }
    }
}