using QFramework;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.UI;
using UnityEngine;

namespace TPL.PVZR.Architecture.Managers
{
    public class GameStarter : MonoBehaviour,IController
    {
        void Start()
        {
            // 初始启动
            this.GetSystem<IGamePhaseSystem>();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}