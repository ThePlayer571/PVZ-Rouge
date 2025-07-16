using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using TPL.PVZR.ViewControllers.Entities.Zombies.View.Components;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.View.BaseView
{
    public abstract class ZombieViewController : MonoBehaviour
    {
        #region 字段

        [SerializeField] protected Zombie Zombie;
        [SerializeField] protected List<ZombieComponentView> zombieComponentViews;
        [SerializeField] protected List<ZombieArmorView> zombieArmorViews;

        protected FSM<ZombieState> ViewFSM;
        protected Animator _Animator;

        #endregion

        #region Unity生命周期

        private void Awake()
        {
            _Animator = GetComponent<Animator>();
            ViewFSM = new FSM<ZombieState>();
        }

        private void Start()
        {
            Zombie.OnInitialized.Register(Initialize).UnRegisterWhenGameObjectDestroyed(this);
        }

        private void Update()
        {
            // 跟踪状态机
            if (ViewFSM.CurrentStateId != Zombie.FSM.CurrentStateId)
            {
                ViewFSM.ChangeState(Zombie.FSM.CurrentStateId);
            }

            ViewFSM.Update();

            OnUpdate();
        }

        #endregion

        #region 初始化

        private void Initialize()
        {
            // 初始化状态机
            SetUpFSM();

            // 分配SortingLayer
            var baseOrder = Zombie.AllocateSortingLayer();
            foreach (var zombieComponentView in zombieComponentViews)
                zombieComponentView.SpriteRenderer.sortingOrder += baseOrder;
            foreach (var zombieArmorView in zombieArmorViews.Where(z => z != null))
                zombieArmorView.SpriteRenderer.sortingOrder += baseOrder;

            //
            OnInit();
        }

        #endregion

        #region Overridable

        protected virtual void OnInit()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void SetUpFSM()
        {
        }

        #endregion
    }
}