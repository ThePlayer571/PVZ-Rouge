using System;
using TPL.PVZR.Classes.DataClasses.Attack;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.ZombieArmor
{
    public class ZombieArmorView : ZombieComponentView
    {
        [SerializeField] private Sprite intactArmor;
        [SerializeField] private Sprite damagedArmor;
        [SerializeField] private Sprite brokenArmor;
        
        public void ChangeArmorStateByAttack(ArmorState armorState, AttackData attackData)
        {
            switch (armorState)
            {
                case ArmorState.Intact:
                    SpriteRenderer.sprite = intactArmor;
                    break;
                case ArmorState.Damaged:
                    SpriteRenderer.sprite = damagedArmor;
                    break;
                case ArmorState.Broken:
                    SpriteRenderer.sprite = brokenArmor;
                    break;
                case ArmorState.Destroyed:
                    DisassembleWithForce(attackData.Punch(transform.position));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(armorState), armorState, null);
            }
        }
    }
}