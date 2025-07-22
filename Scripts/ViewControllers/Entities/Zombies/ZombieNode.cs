using TPL.PVZR.Tools;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Base
{
    public sealed class ZombieNode : MonoBehaviour
    {
        [SerializeField] public ZombieAttackAreaController AttackArea;
        [SerializeField] public Transform JumpDetectionPoint;
        [SerializeField] public CollisionDetector ClimbDetector;
        [SerializeField] public Transform MassCenter;
        [SerializeField] public Transform CorePos;
        [SerializeField] public Transform HeadPos;
    }
}