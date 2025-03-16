using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

namespace TPL.PVZR
{

    public class StartGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<ILevelSystem>().EnterLevel(new LevelDaveHouse());
            // ActionKit.DelayFrame(1, () =>
            // {
            //     this.GetModel<IGameModel>().OnEnterGameSceneInit();
            //     this.SendEvent<EnterGameSceneInitEvent>();
            //
            // }).Start(GameManager.Instance);
        }
    }

    public class ZombieDeadCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            
        }
    }
}