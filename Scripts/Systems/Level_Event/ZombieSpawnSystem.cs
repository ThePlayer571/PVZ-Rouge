using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.ZombieSpawner;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.Systems
{
    public interface IZombieSpawnSystem : ISystem
    {
        public int ActiveTasksCount { get; }
    }

    public class ZombieSpawnSystem : AbstractSystem, IZombieSpawnSystem
    {
        private List<ZombieSpawnTask> ActiveTasks;
        private Timer SpawnTimer;

        private void Update()
        {
            SpawnTimer.Update(Time.deltaTime);
            if (SpawnTimer.Ready && ActiveTasks.Count > 0)
            {
                SpawnTimer.SetRemaining(RandomHelper.Default.Range(0.5f, 1f));
                foreach (var task in ActiveTasks.ToList())
                {
                    if (task.IsFinished)
                    {
                        ActiveTasks.Remove(task);
                        continue;
                    }

                    var info = task.GetRandomZombieSpawnInfo();
                    if (info != null) Spawn(info);
                }
            }
        }

        private void Spawn(ZombieSpawnInfo info)
        {
            EntityFactory.ZombieFactory.SpawnZombie(info.ZombieId, info.SpawnPosition);
        }

        private void Reset()
        {
        }


        private ILevelModel _LevelModel;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();

            ActiveTasks = new List<ZombieSpawnTask>();
            SpawnTimer = new Timer(0.5f);

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.Gameplay:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                GameManager.ExecuteOnUpdate(Update);
                                break;
                            case PhaseStage.LeaveNormal:
                                GameManager.StopOnUpdate(Update);
                                Reset();
                                break;
                        }

                        break;
                }
            });

            this.RegisterEvent<OnWaveStart>(e =>
            {
                var infos = _LevelModel.LevelData.ZombieSpawnInfosOfWave(e.Wave);
                var value = _LevelModel.LevelData.ValueOfWave(e.Wave);
                var task = new ZombieSpawnTask(infos, value);
                ActiveTasks.Add(task);
            });
        }

        public int ActiveTasksCount => ActiveTasks.Count;
    }
}