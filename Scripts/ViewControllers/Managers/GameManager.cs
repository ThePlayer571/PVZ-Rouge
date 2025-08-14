using System;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Systems.Level_Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPL.PVZR.ViewControllers.Managers
{
    public class GameManager : MonoSingleton<GameManager>, ICanGetModel, ICanGetSystem, ICanSendCommand, ICanSendEvent,
        ICanGetService
    {
        private void OnGUI()
        {
            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 10, 120, 40), "Pos1生成僵尸"))
            {
                var pos = LevelGridHelper.CellToWorldBottom(new Vector2Int(22, 9));
                var zombieService = this.GetService<IZombieService>();
                zombieService.SpawnZombie(ZombieId.NewspaperZombie, pos);
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 60, 120, 40), "获取500阳光"))
            {
                this.GetModel<ILevelModel>().SunPoint.Value += 500;
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 110, 120, 40), "重置冷却时间"))
            {
                foreach (var seed in this.GetModel<ILevelModel>().ChosenSeeds)
                {
                    seed.ColdTimeTimer.SetRemaining(0);
                    this.GetModel<ILevelModel>().SunPoint.Value += 25;
                }
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 160, 120, 40), "杀死所有僵尸"))
            {
                var zombieService = this.GetService<IZombieService>();
                var list = zombieService.ActiveZombies.ToList();
                foreach (var zombie in list)
                {
                    zombie.Kill();
                }
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 210, 120, 40), "开始波次1"))
            {
                this.SendEvent<OnWaveStart>(new OnWaveStart { Wave = 6 });
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 260, 120, 40), "获得100硬币"))
            {
                this.GetModel<IGameModel>().GameData.InventoryData.Coins.Value += (1000000000);
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 310, 120, 40), "获取一张豌豆射手"))
            {
                var _ = ItemCreator.CreateCardData(new PlantDef { Id = PlantId.PeaShooter, Variant = PlantVariant.V0 });
                this.GetModel<IGameModel>().GameData.InventoryData.AddCard(_);
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 360, 120, 40), fast ? "恢复正常速度" : "时间流速加快"))
            {
                if (fast)
                {
                    Time.timeScale = 1;
                    fast = false;
                }
                else
                {
                    Time.timeScale = 100;
                    fast = true;
                }
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 410, 120, 40), invulnerable ? "解除无敌" : "玩家无敌"))
            {
                invulnerable = !invulnerable;
            }

            if (UnityEngine.GUI.Button(new UnityEngine.Rect(10, 460, 120, 40), "音效测试"))
            {
                this.GetService<IAudioService>().PlaySFX("event:/TestEvent");
            }

            // if (Input.GetKeyDown(KeyCode.LeftShift))
            // {
            //     var LevelGridModel = this.GetModel<ILevelGridModel>();
            //     var handPos = HandHelper.HandCellPos();
            //     var handCell = LevelGridModel.GetCell(handPos);
            //     $"手所在的Cell信息：pos: {handPos}, CellTileState: {handCell.CellTileState}, PlantState: {handCell.CellTileState}"
            //         .LogInfo();
            // }

            // if (Input.GetKeyDown(KeyCode.LeftShift))
            // {
            //     var handPos = HandHelper.HandCellPos();
            //     var ZombieAISystem = this.GetSystem<IZombieAISystem>();
            //     ZombieAISystem.ZombieAIUnit.DebugLogCluster(handPos);
            // }
            // if (Input.GetKeyDown(KeyCode.LeftShift))
            // {
            //     $"GameUI: {InputManager.Instance.InputActions.GameUI.enabled}, Level: {InputManager.Instance.InputActions.Level.enabled}"
            //         .LogInfo();
            // }
        }

        private bool fast;
        public bool invulnerable;


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