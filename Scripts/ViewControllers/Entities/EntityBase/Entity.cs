using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Helpers.New;
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
        // TODO 绝对不能让id这么轻易被更改，但是Factory要用（以后改成initialize统一设置）
        public int EntityId { get; set; } = EntityIdHelper.AllocateId();

        #endregion

        #region Unity生命周期

        protected virtual void Awake()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _PhaseModel = this.GetModel<IPhaseModel>();

            _Rigidbody2D = this.GetComponent<Rigidbody2D>();
        }

        #endregion

        #region 实体生命周期

        #region 死亡

        public virtual void Kill()
        {
            DieWith(AttackCreator.CreateAttackData(AttackId.Void));
        }

        /// <summary>
        /// 将实体致死（自然的死亡，与Spawn对应）
        /// </summary>
        public abstract void DieWith(AttackData attackData);

        #endregion

        #region 日常

        protected virtual void Update()
        {
            // 以后可能会把OnUpdate等放到这里
        }

        #endregion

        #endregion

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public virtual Vector2 CoreWorldPos => transform.position;
        public Vector2Int CellPos => LevelGridHelper.WorldToCell(this.transform.position);
    }
}