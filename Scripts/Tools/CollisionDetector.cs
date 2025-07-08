using System;
using QFramework;
using Unity.Collections;
using UnityEngine;

namespace TPL.PVZR.Tools
{
    public class CollisionDetector : MonoBehaviour
    {
        // Public
        public bool HasTarget => _targetCount > 0;
        public EasyEvent<int> OnTargetCountChanged = new EasyEvent<int>();

        // 判别函数，返回true表示该碰撞体是目标
        public Func<Collider2D, bool> TargetPredicate { get; set; }

        [SerializeField, ReadOnly] private int _targetCount = 0;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (TargetPredicate == null || TargetPredicate(other))
            {
                _targetCount++;
                OnTargetCountChanged.Trigger(_targetCount);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (TargetPredicate == null || TargetPredicate(other))
            {
                _targetCount--;
                OnTargetCountChanged.Trigger(_targetCount);
            }
        }
    }
}