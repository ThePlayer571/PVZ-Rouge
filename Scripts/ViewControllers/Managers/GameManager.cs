using System;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Helpers.Factory;
using TPL.PVZR.Helpers.Methods;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPL.PVZR.ViewControllers.Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private void OnGUI()
        {
            // TODO 创造一个按钮，按下后调用一个函数
            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 10, 120, 40), "测试按钮"))
            {
                var pos = LevelGridHelper.CellToWorldBottom(new Vector2Int(32, 9));
                EntityFactory.ZombieFactory.SpawnZombie(ZombieId.NormalZombie, pos);
            }
        }


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
            UpdateUIState();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }


        public static bool IsPointerOverUI { get; private set; }

        // 在主控制器Update里每帧调用
        private static void UpdateUIState()
        {
            IsPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}