using QFramework;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.Others.MazeMap;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
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

        private ISceneTransitionEffectService _SceneTransitionEffectService;
        private IPhaseService _PhaseService;

        private AsyncOperationHandle<SceneInstance> _MazeMapSceneHandle;

        public IMazeMapController _MazeMapController { get; private set; }

        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();
            _PhaseModel = this.GetModel<IPhaseModel>();
            _SceneTransitionEffectService = this.GetService<ISceneTransitionEffectService>();
            _PhaseService = this.GetService<IPhaseService>();


            _PhaseService.RegisterCallBack((GamePhase.GameInitialization, PhaseStage.EnterNormal), e =>
            {
                _MazeMapController = MazeMapController.Create(_GameModel.GameData.MazeMapData);
                // MazeController数据结构生成
                _MazeMapController.GenerateMazeMatrix();
                _MazeMapController.LoadTombDataFromMazeMapData();
            });

            _PhaseService.RegisterCallBack((GamePhase.LevelPassed, PhaseStage.EnterNormal),
                e => { _MazeMapController.BreakTomb(_GameModel.ActiveTombData); });

            _PhaseService.RegisterCallBack((GamePhase.MazeMapInitialization, PhaseStage.EnterEarly), e =>
            {
                _MazeMapSceneHandle = Addressables.LoadSceneAsync("MazeMapScene");
                _PhaseService.AddAwait(_MazeMapSceneHandle.Task);
            });

            _PhaseService.RegisterCallBack((GamePhase.MazeMapInitialization, PhaseStage.EnterNormal), e =>
            {
                // 设置场景
                _MazeMapController.SetUpView();
                // 设置摄像头位置
                var matrixPos = _MazeMapController.GetCurrentMatrixPos();
                var tilemapPos = _MazeMapController.MatrixToTilemapPosition(matrixPos);
                var worldPos = MazeMapTilemapController.Instance.Ground.CellToWorld(
                    new Vector3Int(tilemapPos.x, tilemapPos.y, 0));

                Camera.main.Position2D(worldPos);
                // 切换状态
                _PhaseService.ChangePhase(GamePhase.MazeMap);
            });

            _PhaseService.RegisterCallBack((GamePhase.MazeMapInitialization, PhaseStage.EnterLate), e =>
            {
                //
                UIKit.OpenPanel<UIMazeMapPanel>();
            });

            _PhaseService.RegisterCallBack((GamePhase.MazeMap, PhaseStage.EnterLate), e =>
            {
                var task = _SceneTransitionEffectService.EndTransition();
                _PhaseService.AddAwait(task);
            });

            _PhaseService.RegisterCallBack((GamePhase.MazeMap, PhaseStage.LeaveEarly), e =>
            {
                var task = _SceneTransitionEffectService.PlayTransitionAsync();
                _PhaseService.AddAwait(task);
            });

            _PhaseService.RegisterCallBack((GamePhase.MazeMap, PhaseStage.LeaveNormal), e =>
            {
                UIKit.ClosePanel<UIMazeMapPanel>();
                ActionKit.Sequence()
                    .Condition(() => SceneManager.GetSceneByName("MazeMapScene") == null)
                    .Callback(() => _MazeMapSceneHandle.Release())
                    .StartGlobal();
            });
            _PhaseService.RegisterCallBack((GamePhase.GameExiting, PhaseStage.LeaveLate), e =>
            {
                _MazeMapController.TempReleaseHandles();
                _MazeMapController = null;
            });
        }
    }
}