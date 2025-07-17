using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Helpers.New.Methods;
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
            DieWith(AttackCreator.CreateAttackData(AttackId.Void));
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

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public virtual Vector2 CoreWorldPos => transform.position;
        public Vector2Int CellPos => LevelGridHelper.WorldToCell(this.transform.position);
    }
}