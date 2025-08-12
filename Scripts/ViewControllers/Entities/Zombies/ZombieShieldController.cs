using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.ViewControllers.Entities.EntityBase.Interfaces;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies
{
    public class ZombieShieldController : MonoBehaviour, IAttackable
    {
        [SerializeField] private Zombie Zombie;

        private IHaveShield target;

        private void Start()
        {
            target = Zombie.GetComponent<IHaveShield>();
            if (target == null) throw new Exception($"僵尸未实现 IHaveShield: {nameof(Zombie)}");

            Zombie.OnInitialized.Register(() =>
            {
                target.ShieldArmorData.OnDestroyed.Register(() => gameObject.DestroySelf())
                    .UnRegisterWhenGameObjectDestroyed(this);
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public AttackData TakeAttack(AttackData attackData)
        {
            return target.ShieldTakeAttack(attackData);
        }
    }
}