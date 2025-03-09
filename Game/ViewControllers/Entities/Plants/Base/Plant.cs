using QFramework;
using UnityEngine;

namespace TPL.PVZR.EntityPlant
{
    public interface IPlant : IEntity,IDealAttack
    {
    }

    public enum PlantState
    {
        Idle, Dead
    }
    public abstract class Plant : Entity, IPlant
    {
        /// <summary>
        /// Behavior
        /// </summary>
        
        // 植物属性
        protected Direction2 direction;
        protected BindableProperty<float> healthPoint;
        protected FSM<PlantState> behaviorState = new();


        #region 植物行为(方法)

        public void DealAttack(Attack attack)
        {
            this.healthPoint.Value -= attack.damage;
        }

        public override void Kill()
        {
            healthPoint.Value = 0;
        }

        protected virtual void Dead()
        {
            _GameModel.CellGrid[gridPos2.x, gridPos2.y].cellState = Cell.CellState.Empty;
            _GameModel.CellGrid[gridPos2.x, gridPos2.y].plant = null;
            gameObject.DestroySelf();
        }
        

        #endregion
        # region 植物行为(逻辑)
        protected virtual void DefaultAI()
        {
        }

        protected virtual void SetUpState()
        {
            behaviorState.State(PlantState.Idle)
                .OnUpdate(() =>
                {
                    DefaultAI();
                });
            behaviorState.State(PlantState.Dead)
                .OnEnter(() =>
                {
                    Dead();
                });
            behaviorState.StartState(PlantState.Idle);
        }
    # endregion
        protected virtual void Update()
        {
            behaviorState.Update();
        }
        // 初始化
        public virtual void Initialize(Direction2 direction)
        {
            this.direction = direction;
            gameObject.LocalScaleX(direction == Direction2.Right ? 1 : -1);
        }
        
        /// <summary>
        /// Code
        /// </summary>
        
        // 属性
        protected Vector2 directionVector => direction == Direction2.Right ? Vector2.right : Vector2.left;
        // 初始化
        protected override void Awake()
        {
            base.Awake();
            //
            healthPoint = new BindableProperty<float>(100);
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
/*
/// <summary>
/// Behavior
/// </summary>
        
// 植物属性

// 植物行为

// 初始化
*/