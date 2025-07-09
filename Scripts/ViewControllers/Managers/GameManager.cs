using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPL.PVZR.ViewControllers.Managers
{
    public class GameManager : MonoSingleton<GameManager>, ICanGetModel
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

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 160, 120, 40), "测试按钮"))
            {
                var pos = LevelGridHelper.CellToWorldBottom(new Vector2Int(32, 9));
                EntityFactory.ZombieFactory.SpawnZombie(ZombieId.ConeheadZombie, pos);
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 210, 120, 40), "戴夫位置生成阳光"))
            {
                var pos = ReferenceHelper.Player.transform.position;
                EntityFactory.SunFactory.SpawnSunWithFall(pos);
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 260, 120, 40), "杀死所有僵尸"))
            {
                var list = EntityFactory.ZombieFactory.ActiveZombies.ToList();
                foreach (var zombie in list)
                {
                    zombie.Kill();
                }
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 310, 120, 40), "开始游戏"))
            {
                var _PhaseModel = this.GetModel<IPhaseModel>();
                var _GameModel = this.GetModel<IGameModel>();
                _PhaseModel.DelayChangePhase(GamePhase.LevelPreInitialization,
                    new Dictionary<string, object>
                        { { "LevelData", LevelHelper.CreateLevelData(_GameModel.GameData, LevelId.Dave_Lawn) } });
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                var LevelGridModel = this.GetModel<ILevelGridModel>();
                var handPos = HandHelper.HandCellPos();
                var handCell = LevelGridModel.GetCell(handPos);
                $"手所在的Cell信息：pos: {handPos}, TileState: {handCell.CellTileState}, PlantState: {handCell.CellPlantState}"
                    .LogInfo();
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

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}