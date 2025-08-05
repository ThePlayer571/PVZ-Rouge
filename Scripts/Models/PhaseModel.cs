using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Services;
using UnityEngine;

namespace TPL.PVZR.Models
{
    public interface IPhaseModel : IModel
    {
        GamePhase GamePhase { get; set; }
        bool IsInRoughPhase(RoughPhase roughPhase);
    }

    public class PhaseModel : AbstractModel, IPhaseModel
    {
        public GamePhase GamePhase { get; set; }
        protected override void OnInit()
        {
            GamePhase = GamePhase.BeforeStart;
        }


        public bool IsInRoughPhase(RoughPhase roughPhase)
        {
            return roughPhase switch
            {
                RoughPhase.Game => GamePhase is GamePhase.GameInitialization or GamePhase.MazeMapInitialization
                    or GamePhase.MazeMap or GamePhase.LevelPreInitialization or GamePhase.LevelInitialization
                    or GamePhase.ChooseSeeds or GamePhase.ReadyToStart or GamePhase.Gameplay or GamePhase.AllEnemyKilled
                    or GamePhase.LevelExiting or GamePhase.LevelInterrupted or GamePhase.LevelPassed
                    or GamePhase.LevelDefeat or GamePhase.LevelDefeatPanel,
                RoughPhase.Level => GamePhase is GamePhase.LevelPreInitialization or GamePhase.LevelInitialization
                    or GamePhase.ChooseSeeds or GamePhase.ReadyToStart or GamePhase.Gameplay or GamePhase.AllEnemyKilled
                    or GamePhase.LevelExiting or GamePhase.LevelInterrupted or GamePhase.LevelPassed
                    or GamePhase.LevelDefeat or GamePhase.LevelDefeatPanel,
                RoughPhase.Loading => GamePhase is GamePhase.GameInitialization or GamePhase.MazeMapInitialization
                    or GamePhase.LevelPreInitialization or GamePhase.LevelInitialization or GamePhase.LevelInterrupted
                    or GamePhase.LevelPassed or GamePhase.LevelDefeat or GamePhase.LevelExiting
                    or GamePhase.GameExiting,
                _ => throw new ArgumentOutOfRangeException(nameof(roughPhase), roughPhase, null)
            };
        }
    }


    public enum RoughPhase
    {
        Game,
        Level,
        Loading
    }
}