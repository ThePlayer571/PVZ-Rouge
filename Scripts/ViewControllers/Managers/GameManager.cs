using System;
using QFramework;

namespace TPL.PVZR.ViewControllers.Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public static event Action OnUpdate;
        public static event Action OnFixedUpdate;

        public static void ExecuteOnUpdate(Action func)
        {
            OnUpdate += func;
        }

        public static void StopOnUpdate(Action func)
        {
            OnUpdate -= func;
        }

        public static void ExecuteOnFixedUpdate(Action func)
        {
            OnFixedUpdate += func;
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
    }
}