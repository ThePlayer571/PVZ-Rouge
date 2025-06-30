using System;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.Methods;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPL.PVZR.ViewControllers.Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private void OnGUI()
        {
            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 10, 120, 40), "平地"))
            {
                var pos = LevelGridHelper.CellToWorldBottom(new Vector2Int(32, 9));
                EntityFactory.ZombieFactory.SpawnZombie(ZombieId.NormalZombie, pos);
            }
            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 60, 120, 40), "二楼"))
            {
                var pos = LevelGridHelper.CellToWorldBottom(new Vector2Int(14, 14));
                EntityFactory.ZombieFactory.SpawnZombie(ZombieId.NormalZombie, pos);
            }
            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 110, 120, 40), "屋顶"))
            {
                var pos = LevelGridHelper.CellToWorldBottom(new Vector2Int(10, 23));
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