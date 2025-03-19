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

        public void Start()
        {
            this.RegisterEvent<EnterGameSceneInitEvent>(@event =>
            {
                ActionKit.Repeat(1)
                    .Callback(() =>
                    {
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