using QFramework;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.Others.MazeMap;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPL.PVZR.Systems
{
    public interface IMazeMapSystem : IMainSystem
    {
        IMazeMapController _MazeMapController { get; }
    }

    public class MazeMapSystem : AbstractSystem, IMazeMapSystem
    {
        private IGameModel _GameModel;
        private IPhaseModel _PhaseModel;

        public IMazeMapController _MazeMapController { get; private set; }


        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();
            _PhaseModel = this.GetModel<IPhaseModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.GameInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                _MazeMapController = MazeMapController.Create(_GameModel.GameData.MazeMapData);
                                // MazeController数据结构生成
                                _MazeMapController.GenerateMazeMatrix();
                                // 初始化关卡数据（以后改成检测是否已经初始化的）
                                _MazeMapController.InitializeTombData();
                                break;
                        }

                        break;

                    case GamePhase.LevelExiting:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                _MazeMapController.BreakTomb(_GameModel.ActiveTombData);
                                break;
                        }

                        break;

                    case GamePhase.MazeMapInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterEarly:
                                // 加载场景
                                SceneManager.LoadScene("MazeMapScene");

                                // UI界面
                                UIKit.OpenPanel<UIMazeMapPanel>();

                                break;
                            case PhaseStage.EnterNormal:
                                ActionKit.Sequence()
                                    .DelayFrame(1) // 等待场景加载
                                    .Callback(() =>
                                    {
                                        // 设置场景
                                        _MazeMapController.SetUpView();
                                        // 设置摄像头位置
                                        var matrixPos = _MazeMapController.GetCurrentMatrixPos();
                                        var tilemapPos = _MazeMapController.MatrixToTilemapPosition(matrixPos);
                                        var worldPos = MazeMapTilemapController.Instance.Ground.CellToWorld(
                                            new Vector3Int(tilemapPos.x, tilemapPos.y, 0));

                                        Camera.main.Position2D(worldPos);
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