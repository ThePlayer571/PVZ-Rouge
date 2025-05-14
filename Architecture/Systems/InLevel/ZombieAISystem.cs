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
using TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit;
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
                    IZombieAIUnit _ = new ZombieAIUnit();
                    _.InitializeFromMap();
                }
            });
        }

        public Matrix<Vertex> matrix { get; private set; }
    }
}