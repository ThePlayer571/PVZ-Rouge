using System;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies
{
    public class ZombieViewController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour Zombie;

        private void Awake()
        {
            var trmp =
                Zombie as IZombie;
        }
    }
}