using System;
using System.Collections;
using System.ComponentModel;
using QFramework;
using UnityEngine;

namespace TPL.PVZR
{
    public interface IEntity
    {
        public GameObject gameObject { get; }

        public void Kill();
    }

    public abstract class Entity : ViewController, IEntity, IController
    {
        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return PVZRouge.Interface;
        }
        // 属性(对内外都可用)
        public new GameObject gameObject => this.transform.gameObject;
        
        public Vector3Int gridPos => _LevelModel.Grid.WorldToCell(transform.position);

        public Vector2Int gridPos2 => new Vector2Int(gridPos.x, gridPos.y);

        public Cell currentCell => _LevelModel.CellGrid[gridPos.x, gridPos.y];
        // 方法
        public virtual void Kill()
        {
            
        }

        # region 碰撞

        //
        protected event Action<Collision2D> OnCollisionEnterEvent;
        protected event Action<Collision2D> OnCollisionStayEvent;
        protected event Action<Collision2D> OnCollisionExitEvent;
        protected event Action<Collider2D> OnTriggerEnterEvent;
        protected event Action<Collider2D> OnTriggerStayEvent;
        protected event Action<Collider2D> OnTriggerExitEvent;
        //

        protected void OnCollisionEnter2D(Collision2D other)
        {
            OnCollisionEnterEvent?.Invoke(other);
        }

        protected void OnCollisionStay2D(Collision2D other)
        {
            OnCollisionStayEvent?.Invoke(other);
        }

        protected void OnCollisionExit2D(Collision2D other)
        {
            OnCollisionExitEvent?.Invoke(other);
        }

        protected void OnTriggerStay2D(Collider2D other)
        {
            OnTriggerEnterEvent?.Invoke(other);
        }

        protected void OnTriggerEnter2D(Collider2D other)
        {
            OnTriggerStayEvent?.Invoke(other);
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            OnTriggerExitEvent?.Invoke(other);
        }

        #endregion

        // 引用
        protected IEntityCreateSystem _EntityCreateSystem;
        protected ILevelModel _LevelModel;

        protected virtual void Awake()
        {
            _EntityCreateSystem = this.GetSystem<IEntityCreateSystem>();
            _LevelModel = this.GetModel<ILevelModel>();
        }
    }
}