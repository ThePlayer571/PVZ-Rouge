using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Attack;
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
        }

        public AttackData TakeAttack(AttackData attackData)
        {
            target.ShieldTakeAttack(attackData);
            return null;
        }
    }
}