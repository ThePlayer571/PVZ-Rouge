using QFramework;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Attack;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.EntityBase
{
    public abstract class Entity : MonoBehaviour, IEntity
    {
        #region 字段

        protected ILevelModel _LevelModel { get; private set; }
        protected ILevelData LevelData => _LevelModel.LevelData;
        protected IPhaseModel _PhaseModel { get; private set; }
        protected GlobalEntityData GlobalEntityData => _LevelModel.LevelData.GlobalEntityData;

        //
        public Rigidbody2D _Rigidbody2D { get; private set; }

        #endregion

        #region Unity生命周期

        protected virtual void Awake()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _PhaseModel = this.GetModel<IPhaseModel>();

            _Rigidbody2D = this.GetComponent<Rigidbody2D>();
        }

        protected virtual void Update()
        {
        }

        #endregion

        #region 实体生命周期

        public virtual void Kill()
        {
            DieWith(AttackHelper.CreateAttackData(AttackId.Void));
        }

        /// <summary>
        /// 将实体致死（自然的死亡，与Spawn对应）
        /// </summary>
        public virtual void DieWith(AttackData attackData)
        {
            Remove();
        }

        // todo 应该：实体不具备remove函数（实体只处理形象的逻辑，比如Diewith）
        // 尽量放到Factory里面处理（不过暂时不需要）
        /// <summary>
        /// 将实体从场景中移除（不包含动画/事件等）
        /// </summary>
        public virtual void Remove()
        {
            gameObject.DestroySelf();
        }

        #endregion

        #region 碰撞事件

        public EasyEvent<Collider2D> OnTriggerEnterEvent = new EasyEvent<Collider2D>();
        public EasyEvent<Collider2D> OnTriggerExitEvent = new EasyEvent<Collider2D>();
        public EasyEvent<Collider2D> OnTriggerStayEvent = new EasyEvent<Collider2D>();
        public EasyEvent<Collision2D> OnCollisionEnter2DEvent = new EasyEvent<Collision2D>();
        public EasyEvent<Collision2D> OnCollisionStay2DEvent = new EasyEvent<Collision2D>();
        public EasyEvent<Collision2D> OnCollisionExit2DEvent = new EasyEvent<Collision2D>();

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTriggerEnterEvent.Trigger(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnTriggerExitEvent.Trigger(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            OnTriggerStayEvent.Trigger(other);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            OnCollisionEnter2DEvent.Trigger(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            OnCollisionStay2DEvent.Trigger(other);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            OnCollisionExit2DEvent.Trigger(other);
        }

        #endregion

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public virtual Vector2 CoreWorldPos => transform.position;
        public Vector2Int CellPos => LevelGridHelper.WorldToCell(this.transform.position);
    }
}