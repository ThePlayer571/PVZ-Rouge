using System;
using System.Collections.Generic;
using QFramework;
using Unity.Collections;
using UnityEngine;

namespace TPL.PVZR.Tools
{
    public class CollisionDetector : MonoBehaviour
    {
        private HashSet<Collider2D> _detectedTargets { get; } = new HashSet<Collider2D>();

        // Public
        public bool HasTarget => _targetCount > 0;
        public readonly EasyEvent<int> OnTargetCountChanged = new EasyEvent<int>();
        [SerializeField] public bool RecordTargets = false;
        public IReadOnlyCollection<Collider2D> DetectedTargets => _detectedTargets;

        // 判别函数，返回true表示该碰撞体是目标
        public Func<Collider2D, bool> TargetPredicate { get; set; }

        [SerializeField, ReadOnly] private int _targetCount = 0;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (TargetPredicate == null || TargetPredicate(other))
            {
                _targetCount++;
                if (RecordTargets)
                {
                    _detectedTargets.Add(other);
                }

                OnTargetCountChanged.Trigger(_targetCount);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (TargetPredicate == null || TargetPredicate(other))
            {
                _targetCount--;
                _detectedTargets.Remove(other);
                OnTargetCountChanged.Trigger(_targetCount);
            }
        }
    }
}