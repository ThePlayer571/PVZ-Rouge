using System;
using System.Collections.Generic;
using QFramework;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Tools
{
    public class TriggerDetector : MonoBehaviour
    {
        #region Public

        // 属性
        public bool HasTarget => _targetCount > 0 || DebugAlwaysReturnHasTarget;
        public int TargetCount => _targetCount;

        public bool RecordTargets { get; set; }

        public IReadOnlyCollection<Collider2D> DetectedTargets => _detectedTargets;

        public Func<Collider2D, bool> TargetPredicate { get; set; } // 判别函数，只记录返回值为true的target

        // 事件
        public readonly EasyEvent<int> OnTargetCountChanged = new EasyEvent<int>();
        public readonly EasyEvent<Collider2D> OnTargetEnter = new EasyEvent<Collider2D>();
        public readonly EasyEvent<Collider2D> OnTargetExit = new EasyEvent<Collider2D>();

        #endregion

        private HashSet<Collider2D> _detectedTargets { get; } = new HashSet<Collider2D>();

         private int _targetCount = 0;

        [SerializeField] public bool DebugAlwaysReturnHasTarget = false; // Renamed from DEBUG_AlwaysReturnHasTarget

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (TargetPredicate == null || TargetPredicate(other))
            {
                _targetCount++;
                if (RecordTargets)
                {
                    _detectedTargets.Add(other);
                }

                OnTargetEnter.Trigger(other);
                OnTargetCountChanged.Trigger(_targetCount);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (TargetPredicate == null || TargetPredicate(other))
            {
                _targetCount--;
                _detectedTargets.Remove(other);

                OnTargetExit.Trigger(other);
                OnTargetCountChanged.Trigger(_targetCount);
            }
        }
    }
}