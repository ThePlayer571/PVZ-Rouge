using QFramework;
using TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
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

            var phaseService = this.GetService<IPhaseService>();
            phaseService.RegisterCallBack((GamePhase.Gameplay, PhaseStage.EnterNormal), e => { StartRunning(); });
            phaseService.RegisterCallBack((GamePhase.Gameplay, PhaseStage.LeaveNormal), e => { StopRunning(); });

            this.RegisterEvent<OnSeedInHandPlanted>(e => { e.PlantedSeed.ColdTimeTimer.Reset(); });
        }
    }
}