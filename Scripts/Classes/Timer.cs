using System;
using UnityEngine;

namespace TPL.PVZR.Classes
{
    [Serializable]
    public class Timer
    {
        [SerializeField]
        private float duration;
        [SerializeField]
        private bool ready = false;
        [SerializeField]
        private float elapsedTime = 0;

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

        public float ElapsedTime
        {
            get => elapsedTime;
            private set => elapsedTime = value;
        }

        public void ChangeDuration(float duration)
        {
            Duration = duration;
        }

        public void Reset()
        {
            ElapsedTime = 0;
            Ready = false;
        }

        public void Update(float deltaTime)
        {
            if (Ready) return;
            ElapsedTime += deltaTime;
            if (ElapsedTime >= Duration) Ready = true;
        }
    }
}
