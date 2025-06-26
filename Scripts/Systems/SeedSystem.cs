using System.Collections.Generic;
using System.Threading;
using QFramework;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.Events;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;
using Timer = TPL.PVZR.Classes.Timer;

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
                                GameManager.ExecuteOnUpdate(Update);
                                break;
                        }

                        break;
                    case GamePhase.LevelExiting:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                GameManager.StopOnUpdate(Update);
                                break;
                        }

                        break;
                }
            });

            this.RegisterEvent<PlantingSeedInHandEvent>(e =>
            {
                e.PlantedSeed.ColdTimeTimer.Reset();
            });
        }
    }
}