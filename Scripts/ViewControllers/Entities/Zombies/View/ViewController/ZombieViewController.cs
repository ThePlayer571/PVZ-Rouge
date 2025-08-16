using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using TPL.PVZR.ViewControllers.Entities.Zombies.View.Components;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.View.ViewController
{
    public abstract class ZombieViewController : MonoBehaviour
    {
        #region 字段

        protected Zombie Zombie;
        protected List<ZombieComponentView> zombieComponentViews = new();
        protected List<ZombieArmorView> zombieArmorViews = new();

        protected FSM<ZombieState> ViewFSM;
        protected Animator _Animator;

        #endregion

        #region Unity生命周期

        private void Awake()
        {
            _Animator = GetComponent<Animator>();
            ViewFSM = new FSM<ZombieState>();

            Zombie = GetComponentInParent<Zombie>();

            foreach (var view in GetComponentsInChildren<ZombieComponentView>(true))
            {
                if (view is ZombieArmorView armor)
                    zombieArmorViews.Add(armor);
                else
                    zombieComponentViews.Add(view);
            }
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
            var baseOrder = EntityIdHelper.AllocateZombieSortingLayer();
            foreach (var zombieComponentView in zombieComponentViews)
                zombieComponentView.SpriteRenderer.sortingOrder += baseOrder;
            foreach (var zombieArmorView in zombieArmorViews.Where(z => z != null))
                zombieArmorView.SpriteRenderer.sortingOrder += baseOrder;
            // 冰冻效果显示

            // 黄油效果显示


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

        private SpriteRenderer[] _spriteRenderersCache;

        protected SpriteRenderer[] GetAllSpriteRenderers()
        {
            _spriteRenderersCache ??= transform.GetComponentsInChildren<SpriteRenderer>(true);
            return _spriteRenderersCache;
        }
    }
}