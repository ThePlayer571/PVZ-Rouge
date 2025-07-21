using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.ZombieSpawner;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.Systems.Level_Event
{
    public interface IZombieSpawnSystem : IAutoUpdateSystem
    {
        public int ActiveTasksCount { get; }
    }

    public class ZombieSpawnSystem : AbstractSystem, IZombieSpawnSystem
    {
        private List<RandomPool<ZombieSpawnInfo, ZombieSpawnInfo>> ActiveTasks;
        private Timer SpawnTimer;

        private void Update()
        {
            SpawnTimer.Update(Time.deltaTime);
            if (SpawnTimer.Ready && ActiveTasks.Count > 0)
            {
                SpawnTimer.SetRemaining(RandomHelper.Default.Range(1f, 2f));
                foreach (var task in ActiveTasks.ToList())
                {
                    if (task.IsFinished)
                    {
                        ActiveTasks.Remove(task);
                        continue;
                    }

                    var info = task.GetRandomOutput();
                    if (info != null) Spawn(info);
                }
            }
        }

        private void Spawn(ZombieSpawnInfo info)
        {
            EntityFactory.ZombieFactory.SpawnZombie(info.ZombieId, info.SpawnPosition);
        }

        private void StartRunning()
        {
            GameManager.ExecuteOnUpdate(Update);
        }

        private void StopRunning()
        {
            GameManager.StopOnUpdate(Update);
            if (ActiveTasks.Count != 0) throw new Exception("ActiveTasks尚未清空为0，ZombieSpawnSystem就尝试停止运行");
        }

        private ILevelModel _LevelModel;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();

            ActiveTasks = new List<RandomPool<ZombieSpawnInfo, ZombieSpawnInfo>>();
            SpawnTimer = new Timer(0.5f);

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.Gameplay:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                StartRunning();
                                break;
                            case PhaseStage.LeaveNormal:
                                StopRunning();
                                break;
                        }

                        break;
                }
            });

            this.RegisterEvent<OnWaveStart>(e =>
            {
                var infos = _LevelModel.LevelData.ZombieSpawnInfosOfWave(e.Wave);
                var value = _LevelModel.LevelData.ValueOfWave(e.Wave);
                var task = new RandomPool<ZombieSpawnInfo, ZombieSpawnInfo>(infos, value, RandomHelper.Default);
                ActiveTasks.Add(task);
            });
        }

        public int ActiveTasksCount => ActiveTasks.Count;
    }
}