using System;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.InLevel;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Class.Tags;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities
{
    public interface IEntity : IController
    {
        public GameObject gameObject { get; }

        public void Kill();
        TagGroup tagGroup { get; }
    }

    public abstract class Entity : ViewController, IEntity
    {
        #region Architecture

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        #endregion

        #region IEntity

        public Vector3Int gridPos => ReferenceModel.Get.Grid.WorldToCell(transform.position);
        public Vector2Int gridPos2 => new Vector2Int(gridPos.x, gridPos.y);
        public Cell currentCell => _LevelModel.CellGrid[gridPos.x, gridPos.y];
        public abstract void Kill();

        public TagGroup tagGroup { get; } = new();

        #endregion

        # region 碰撞事件

        protected event Action<Collision2D> OnCollisionEnterEvent;
        protected event Action<Collision2D> OnCollisionStayEvent;
        protected event Action<Collision2D> OnCollisionExitEvent;
        protected event Action<Collider2D> OnTriggerEnterEvent;
        protected event Action<Collider2D> OnTriggerStayEvent;
        protected event Action<Collider2D> OnTriggerExitEvent;

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

        #region 私有
        
        private void Awake()
        {
            _EntitySystem = this.GetSystem<IEntitySystem>();
            _LevelModel = this.GetModel<ILevelModel>();
            OnAwakeBase();
            OnAwake();
        }
        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        #endregion

        // 常用的引用 (懒得每组件Get一次了)
        protected IEntitySystem _EntitySystem;
        protected ILevelModel _LevelModel;

        #region virtual

        /// <summary>
        /// 由Base使用
        /// </summary>
        protected virtual void OnAwakeBase()
        {
            // do nothing
        }
        /// <summary>
        /// 由sealed class使用
        /// </summary>
        protected virtual void OnAwake()
        {
            // do nothing
        }
        

        #endregion

    }
}