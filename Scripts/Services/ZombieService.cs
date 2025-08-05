using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.Services
{
    public interface IZombieService : IService
    {
        void SpawnZombie(ZombieId id, Vector2 pos);
        void RemoveZombie(Zombie zombie);
        void RemoveAllZombies();

        IReadOnlyCollection<Zombie> ActiveZombies { get; }
    }

    public class ZombieService : AbstractService, IZombieService
    {
        private ZombieFactory _zombieFactory;

        protected override void OnInit()
        {
            _zombieFactory = new ZombieFactory();
        }

        public void SpawnZombie(ZombieId id, Vector2 pos)
        {
            _zombieFactory.SpawnZombie(id, pos);
        }

        public void RemoveZombie(Zombie zombie)
        {
            _zombieFactory.RemoveZombie(zombie);
        }

        public void RemoveAllZombies()
        {
            _zombieFactory.RemoveAllZombies();
        }

        public IReadOnlyCollection<Zombie> ActiveZombies => _zombieFactory.ActiveZombies;
    }
}