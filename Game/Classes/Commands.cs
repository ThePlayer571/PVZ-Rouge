using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

namespace TPL.PVZR
{

    public class StartGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            // 加载场景配置
            SceneManager.LoadScene("TestScene");
            UIKit.OpenPanel<UIGamePanel>();
            UIKit.ClosePanel<UIGameStartPanel>();
            // 系统初始化
            ActionKit.DelayFrame(1, () =>
            {
                this.GetModel<IGameModel>().OnEnterGameSceneInit();
                this.SendEvent<EnterGameSceneInitEvent>();

            }).Start(GameManager.Instance);
        }
    }
}