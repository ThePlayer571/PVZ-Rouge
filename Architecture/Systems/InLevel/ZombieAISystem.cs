using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Class.ZombieAI;
using TPL.PVZR.Gameplay.Class.ZombieAI.Class;
using TPL.PVZR.Gameplay.Class.ZombieAI.PathFinding;
using TPL.PVZR.Gameplay.Class.ZombieAI.Public;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;


namespace TPL.PVZR.Architecture.Systems.InLevel
{
    public interface IZombieAISystem : ISystem
    {
        Matrix<Vertex> matrix { get; }
    }

    // TODO： UnReachable的处理
    public class ZombieAISystem : AbstractSystem, IZombieAISystem
    {
        private ILevelModel _LevelModel;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelInitialization)
                {
                    ZombieAIUnit _ = new ZombieAIUnit();
                    _.InitializeFromMap();
                    _.LogAllKeyAdjacencyList();
                    _.DisplayTheMap();
                    for (int i = 0; i < 100; i++)
                    {
                        AITendency aiTendency = new AITendency(AITendency.MainAI.Default);
                        var path = _.FindPath(_.GetVertex(3, 19), _.GetVertex(13, 16), aiTendency);
                        $"成功返回path{path is not null}".LogInfo();
                        _.LogThePath(path);
                    }
                }
            });
        }

        public Matrix<Vertex> matrix { get; private set; }
    }
}