using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Events;

namespace TPL.PVZR.Test
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