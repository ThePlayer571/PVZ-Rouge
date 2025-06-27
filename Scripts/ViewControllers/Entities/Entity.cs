using System;
using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.Level;
using TPL.PVZR.Core;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities
{
    public class Entity : MonoBehaviour, IEntity
    {
        // 
        protected ILevelModel _LevelModel { get; private set; }
        protected ILevelData LevelData => _LevelModel.LevelData;
        protected IPhaseModel _PhaseModel { get; private set; }
        protected GlobalEntityData GlobalEntityData => _LevelModel.LevelData.GlobalEntityData;

        //
        public Rigidbody2D _Rigidbody2D { get; private set; }

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

        public Vector2Int CellPos => LevelGridHelper.WorldToCell(this.transform.position);
    }
}