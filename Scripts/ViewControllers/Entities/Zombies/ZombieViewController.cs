using System;
using QFramework;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies
{
    public class ZombieViewController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour zombie;
        private Zombie Zombie;

        private void Start()
        {
            Zombie = zombie.GetComponent<Zombie>();

            Zombie.Direction.Register(direction => { Zombie.transform.LocalScaleX(direction.ToInt()); })
                .UnRegisterWhenGameObjectDestroyed(this);
        }


        [SerializeField] private float targetRotation;
        [SerializeField] private float currentRotation;

        [SerializeField] public float factor = 5f;

        private void Update()
        {
            targetRotation = Mathf.Clamp(Mathf.Abs(Zombie._Rigidbody2D.velocity.x), 0, 1) * 10f;
            currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * factor);
            this.transform.LocalEulerAnglesZ(-currentRotation);
        }
    }
}