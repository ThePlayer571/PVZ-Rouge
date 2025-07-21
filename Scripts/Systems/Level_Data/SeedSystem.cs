using QFramework;
using TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.Systems.Level_Data
{
    /// <summary>
    /// Seed的冷却时间更新
    /// </summary>
    public interface ISeedSystem : IAutoUpdateSystem
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
                            case PhaseStage.LeaveNormal:
                                StopRunning();
                                break;
                        }

                        break;
                }
            });

            this.RegisterEvent<OnSeedInHandPlanted>(e => { e.PlantedSeed.ColdTimeTimer.Reset(); });
        }
    }
}