using QFramework;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities
{
    public class Entity : MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}