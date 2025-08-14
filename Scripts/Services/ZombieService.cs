using System.Collections.Generic;
using System.Threading.Tasks;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.Services
{
    public interface IZombieService : IService
    {
        Task SpawnZombie(ZombieId id, Vector2 pos);
        void RemoveZombie(Zombie zombie);
        void RemoveAllZombies();
        void ClearCache();

        IReadOnlyCollection<Zombie> ActiveZombies { get; }
        EasyEvent<int> OnZombieCountChanged { get; }
    }

    public class ZombieService : AbstractService, IZombieService
    {
        private ZombieFactory _zombieFactory;

        protected override void OnInit()
        {
            _zombieFactory = new ZombieFactory();
        }

        public async Task SpawnZombie(ZombieId id, Vector2 pos)
        {
            await _zombieFactory.SpawnZombieAsync(id, pos);
            OnZombieCountChanged.Trigger(_zombieFactory.ActiveZombies.Count);
        }

        public void RemoveZombie(Zombie zombie)
        {
            _zombieFactory.RemoveZombie(zombie);
            OnZombieCountChanged.Trigger(_zombieFactory.ActiveZombies.Count);
        }

        public void RemoveAllZombies()
        {
            _zombieFactory.RemoveAllZombies();
            OnZombieCountChanged.Trigger(_zombieFactory.ActiveZombies.Count);
        }

        public void ClearCache()
        {
            _zombieFactory.ClearCache();
        }

        public IReadOnlyCollection<Zombie> ActiveZombies => _zombieFactory.ActiveZombies;
        public EasyEvent<int> OnZombieCountChanged { get; } = new();
    }
}