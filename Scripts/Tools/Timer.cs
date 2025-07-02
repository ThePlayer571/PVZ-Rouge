using System;
using UnityEngine;

namespace TPL.PVZR.Tools
{
    [Serializable]
    public class Timer
    {
        [SerializeField] private float duration;
        [SerializeField] private bool ready = false;
        [SerializeField] private bool justReady = false; // 是否刚刚变为Ready状态，只维持一帧的true
        [SerializeField] private float elapsedTime = 0;

        public Timer(float duration)
        {
            this.duration = duration;
        }

        public float Duration
        {
            get => duration;
            private set => duration = value;
        }

        public bool Ready
        {
            get => ready;
            private set => ready = value;
        }

        public bool JustReady => justReady;

        public float ElapsedTime
        {
            get => elapsedTime;
            private set => elapsedTime = value;
        }

        public float Remaining => Mathf.Max(0, Duration - ElapsedTime);

        public void SetElapsedTime(float elapsedTime)
        {
            ElapsedTime = elapsedTime;
            UpdateReadyState();
        }

        public void SetRemaining(float remaining)
        {
            ElapsedTime = Duration - remaining;
            UpdateReadyState();
        }

        private void UpdateReadyState()
        {
            bool wasReady = Ready;
            Ready = ElapsedTime >= Duration;
            
            // 如果从非Ready状态变为Ready状态，设置justReady为true
            if (!wasReady && Ready)
            {
                justReady = true;
            }
        }

        public void ChangeDuration(float duration)
        {
            Duration = duration;
        }

        public void Reset()
        {
            ElapsedTime = 0;
            Ready = false;
            justReady = false;
        }

        public void Update(float deltaTime)
        {
            // 每次Update先将justReady重置为false
            justReady = false;
            if (Ready) return;
            ElapsedTime += deltaTime;
            if (ElapsedTime >= Duration)
            {
                Ready = true;
                justReady = true; // 仅在本帧变为Ready时为true
            }
        }
    }
}