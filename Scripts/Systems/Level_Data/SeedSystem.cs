using System.Collections.Generic;
using System.Threading;
using QFramework;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;
using Timer = TPL.PVZR.Tools.Timer;

namespace TPL.PVZR.Systems
{
    /// <summary>
    /// Seed的冷却时间更新
    /// </summary>
    public interface ISeedSystem : ISystem
    {
    }

    public class SeedSystem : AbstractSystem, ISeedSystem
    {
        private void Update()
        {
            foreach (var seedData in _LevelModel.ChosenSeeds)
            {
                seedData.ColdTimeTimer.Update(Time.deltaTime);
            }
        }

        private void StartRunning()
        {
            GameManager.ExecuteOnUpdate(Update);
        }

        private void StopRunning()
        {
            GameManager.StopOnUpdate(Update);
        }

        private ILevelModel _LevelModel;


        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.Gameplay:
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

            this.RegisterEvent<PlantingSeedInHandEvent>(e => { e.PlantedSeed.ColdTimeTimer.Reset(); });
        }
    }
}