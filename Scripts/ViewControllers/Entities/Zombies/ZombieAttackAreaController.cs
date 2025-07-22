using System;
using QFramework;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies
{
    // todo: 历史遗留问题：建议改为ConllisionDetector
    public class ZombieAttackAreaController : MonoBehaviour
    {
        public EasyEvent<Collider2D> OnTargetEnter = new();
        public EasyEvent<Collider2D> OnTargetExit = new();
        public EasyEvent<Collider2D> OnTargetStay = new();
        
        private void OnTriggerEnter2D(Collider2D other)
        {
              OnTargetEnter?.Trigger(other);
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            OnTargetExit?.Trigger(other);
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            OnTargetStay?.Trigger(other);
        }
    }
}