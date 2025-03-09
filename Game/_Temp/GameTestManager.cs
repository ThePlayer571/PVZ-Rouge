using System.Collections.Generic;
using UnityEngine;
using MoonSharp.VsCodeDebugger.SDK;
using QFramework;
using UnityEngine.Tilemaps;

namespace TPL.PVZR
{
    public class GameTestManager : MonoSingleton<GameTestManager>, IController
    {
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        class Test
        {
            public int test;
        }
        public void Start()
        {
            this.RegisterEvent<EnterGameSceneInitEvent>(@event =>
            {
                ActionKit.Repeat(1)
                    .Callback(() =>
                    {
                        this.GetSystem<IEntityCreateSystem>()
                            .CreateZombie(ZombieIdentifier.BucketZombie, new(6, 6));
                        this.GetSystem<IEntityCreateSystem>()
                            .CreateZombie(ZombieIdentifier.ScreenDoorZombie, new(6, 8));
                    })
                    .Delay(2f)
                    .Start(this);







            });

        }
        private void Update()
        {

        }
    }
}