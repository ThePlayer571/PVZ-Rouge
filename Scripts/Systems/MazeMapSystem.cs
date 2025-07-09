using QFramework;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine.SceneManagement;

namespace TPL.PVZR.Systems
{
    public interface IMazeMapSystem : ISystem
    {
    }

    public class MazeMapSystem : AbstractSystem, IMazeMapSystem
    {
        private IGameModel _GameModel;
        private IPhaseModel _PhaseModel;

        private IMazeMapController _MazeMapController;


        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();
            _PhaseModel = this.GetModel<IPhaseModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.MazeMapInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterEarly:
                                // 加载场景
                                SceneManager.LoadScene("MazeMapScene");

                                // MazeController数据结构生成
                                _MazeMapController =
                                    MazeMapController.CreateController(_GameModel.GameData.MazeMapData);
                                _MazeMapController.GenerateMazeMatrix();

                                // UI界面
                                UIKit.OpenPanel<UIMazeMapPanel>();

                                break;
                            case PhaseStage.EnterNormal:
                                ActionKit.Sequence()
                                    .DelayFrame(1) // 等待场景加载
                                    .Callback(() =>
                                    {
                                        // 设置场景
                                        _MazeMapController.SetMazeMapTiles();
                                    })
                                    .Callback(() => { _PhaseModel.DelayChangePhase(GamePhase.MazeMap); })
                                    .Start(GameManager.Instance);
                                break;
                            case PhaseStage.EnterLate:
                                ActionKit.Sequence()
                                    .DelayFrame(2)
                                    .Start(GameManager.Instance);
                                break;
                        }

                        break;
                    case GamePhase.MazeMap:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.LeaveNormal:
                                UIKit.ClosePanel<UIMazeMapPanel>();
                                break;
                        }

                        break;
                }
            });
        }
    }
}