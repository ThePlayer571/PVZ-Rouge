using QFramework;
using TPL.PVZR.Classes.MazeMap.Interfaces;
using TPL.PVZR.Classes.MazeMap.Public.DaveHouse;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers.Managers;
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
                                SceneManager.LoadScene("MazeMapScene");
                                _MazeMapController = new DaveHouseMazeMapController(_GameModel.GameData.MazeMapData);
                                break;
                            case PhaseStage.EnterNormal:
                                ActionKit.Sequence()
                                    .DelayFrame(1) // 等待场景加载
                                    .Callback(() => { _MazeMapController.SetMazeMapTiles(); })
                                    .Start(GameManager.Instance);
                                break;
                            case PhaseStage.EnterLate:
                                ActionKit.Sequence()
                                    .DelayFrame(2)
                                    .Callback(() => { _PhaseModel.DelayChangePhase(GamePhase.MazeMap); })
                                    .Start(GameManager.Instance);
                                break;
                        }

                        break;
                }
            });
        }
    }
}