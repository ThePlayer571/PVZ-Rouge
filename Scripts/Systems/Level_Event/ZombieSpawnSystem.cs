using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.ZombieSpawner;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
using TPL.PVZR.Helpers;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.Others.LevelScene;
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

            void Spawn(ZombieSpawnInfo info)
            {
                _ZombieService.SpawnZombie(info.ZombieId, info.SpawnPosition);
            }
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
        private IZombieService _ZombieService;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _ZombieService = this.GetService<IZombieService>();

            ActiveTasks = new List<RandomPool<ZombieSpawnInfo, ZombieSpawnInfo>>();
            SpawnTimer = new Timer(0.5f);

            var phaseService = this.GetService<IPhaseService>();
            phaseService.RegisterCallBack((GamePhase.Gameplay, PhaseStage.EnterNormal), e => { StartRunning(); });
            phaseService.RegisterCallBack((GamePhase.Gameplay, PhaseStage.LeaveNormal), e => { StopRunning(); });

            this.RegisterEvent<OnWaveStart>(e =>
            {
                var infos = _LevelModel.LevelData.ZombieSpawnInfosOfWave(e.Wave);
                var value = _LevelModel.LevelData.ValueOfWave(e.Wave);
                var task = new RandomPool<ZombieSpawnInfo, ZombieSpawnInfo>(infos, value, RandomHelper.Default);
                ActiveTasks.Add(task);
            });

            this.RegisterEvent<OnFinalWaveStart>(e =>
            {
                if (GravestoneController.Instances.Count > 0)
                {
                    var infos = _LevelModel.LevelData.ZombieSpawnInfosOfWave(_LevelModel.LevelData.TotalWaveCount);
                    var pool = new RandomPool<ZombieSpawnInfo, ZombieSpawnInfo>(infos, 100000, RandomHelper.Default);
                    foreach (var gravestone in GravestoneController.Instances)
                    {
                        var pos = gravestone.transform.position;
                        _ZombieService.SpawnZombie(pool.GetRandomOutput().ZombieId, pos);
                    }
                }
            });
        }

        public int ActiveTasksCount => ActiveTasks.Count;
    }
}