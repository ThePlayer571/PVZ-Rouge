using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
    public class ZombieFactory
    {
        public HashSet<Zombie> ActiveZombies = new();

        public Zombie SpawnZombie(ZombieId id, Vector2 pos)
        {
            var zombiePrefab = ZombieConfigReader.GetZombiePrefab(id);

            var zombie = zombiePrefab.Instantiate(pos, Quaternion.identity).GetComponent<Zombie>();
            zombie.Initialize();

            ActiveZombies.Add(zombie);
            return zombie;
        }

        public void RemoveZombie(Zombie zombie)
        {
            zombie.gameObject.DestroySelf();
            ActiveZombies.Remove(zombie);
        }

        public void RemoveAllZombies()
        {
            foreach (var zombie in ActiveZombies)
            {
                zombie.gameObject.DestroySelf();
            }

            ActiveZombies.Clear();
        }
    }
}