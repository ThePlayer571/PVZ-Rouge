using DG.Tweening;
using UnityEngine;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TPL.PVZR
{
    public interface IZombieSpawnSystem : ISystem, IInLevelSystem
    { 
        void SpawnWaveOfZombie(float value, int wave);
        HashSet<ZombieSpawner> activeZombieSpawners { get; }
    }


    public class ZombieSpawnSystem : AbstractSystem, IZombieSpawnSystem
    {
        private ILevelModel _LevelModel;
        private IEntitySystem _EntitySystem;
        
        //
        public HashSet<ZombieSpawner> activeZombieSpawners { get; private set; }

        public void OnBuildingLevel()
        {
            activeZombieSpawners = new HashSet<ZombieSpawner>();
        }
        public void OnGameplay()
        {
            GameManager.ExecuteOnUpdate(Update);
        }

        public void OnEndGameplay()
        {
            GameManager.StopOnUpdate(Update);
            activeZombieSpawners = null;
        }

        private void Update()
        {
            foreach (var zombieSpawner in activeZombieSpawners.ToArray())
            {
                if (zombieSpawner.finishSpawnTask)
                {
                    activeZombieSpawners.Remove(zombieSpawner);
                }
                else
                {
                    zombieSpawner.Update(Time.deltaTime);
                }
            }
        }

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _EntitySystem = this.GetSystem<IEntitySystem>();
        }

        public void SpawnWaveOfZombie(float value, int wave)
        {
            activeZombieSpawners.Add(new ZombieSpawner(_LevelModel.ZombieSpawnConfig, value, wave));
        }
    }
}