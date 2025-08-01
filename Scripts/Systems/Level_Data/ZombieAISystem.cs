using System.Diagnostics;
using QFramework;
using TPL.PVZR.Classes.ZombieAI.Class;
using TPL.PVZR.Classes.ZombieAI.PathFinding;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.CommandEvents.New.Level_Shit;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.Systems.Level_Data
{
    public interface IZombieAISystem : ISystem
    {
        IZombieAIUnit ZombieAIUnit { get; }
        Vector2Int PlayerVertexPos { get; }
    }

    public class ZombieAISystem : AbstractSystem, IZombieAISystem
    {
        public IZombieAIUnit ZombieAIUnit { get; private set; }
        private ILevelGridModel _LevelGridModel;
        public Vector2Int PlayerVertexPos => new Vector2Int(_playerVertexOnLastFrame.x, _playerVertexOnLastFrame.y);

        private Vertex _playerVertexOnLastFrame;

        private void UpdatePlayerCluster()
        {
            var playerCellPos = Player.Instance.CellPos;
            Vertex playerCurrentVertex = ZombieAIUnit.GetVertexSafely(playerCellPos);
            while (playerCurrentVertex == null && playerCellPos.y > 0)
            {
                playerCellPos.y--;
                playerCurrentVertex = ZombieAIUnit.GetVertexSafely(playerCellPos);
            }

            // 初始化设置
            if (_playerVertexOnLastFrame == null)
            {
                _playerVertexOnLastFrame = playerCurrentVertex;
            }
            // 常规
            else
            {
                // 重新寻路
                var playerLastCluster = ZombieAIUnit.GetClusterSafely(_playerVertexOnLastFrame.Position);

                var playerCurrentCluster = ZombieAIUnit.GetClusterSafely(playerCellPos);

                bool shouldRefindPath =
                    playerCurrentCluster != null && !playerLastCluster.IsIdentical(playerCurrentCluster);
                if (shouldRefindPath)
                {
                    // $"Cluster变化：old: {playerLastCluster}, new: {playerCurrentCluster}"
                    //     .LogInfo();
                    _playerVertexOnLastFrame = playerCurrentVertex;
                    this.SendEvent<OnPlayerChangeCluster>();
                }
            }

            // 更新玩家位置
            if (playerCurrentVertex != null) _playerVertexOnLastFrame = playerCurrentVertex;
        }

        private void StartRunning()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            GameManager.ExecuteOnUpdate(UpdatePlayerCluster);
            ZombieAIUnit = new ZombieAIUnit();
            ZombieAIUnit.InitializeFrom(_LevelGridModel.LevelMatrix);
            
            stopwatch.Stop();
            $"算法耗时：{stopwatch.ElapsedMilliseconds} ms".LogInfo();
            // ZombieAIUnit.DebugDisplayMatrix();
            // ZombieAIUnit.DebugLogCluster(new Vector2Int(11, 14));
        }

        private void StopRunning()
        {
            GameManager.StopOnUpdate(UpdatePlayerCluster);
            ZombieAIUnit = null;
            _playerVertexOnLastFrame = null;
        }

        protected override void OnInit()
        {
            _LevelGridModel = this.GetModel<ILevelGridModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                StartRunning();
                                break;
                        }

                        break;
                    case GamePhase.LevelExiting:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                StopRunning();
                                break;
                        }

                        break;
                }
            });
        }
    }
}