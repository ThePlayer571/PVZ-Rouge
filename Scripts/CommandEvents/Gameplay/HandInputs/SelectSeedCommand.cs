using System;
using QFramework;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;

namespace TPL.PVZR.CommandEvents.Gameplay.HandInputs
{
    public class SelectSeedCommand : AbstractCommand
    {
        public SelectSeedCommand(SeedData seedData)
        {
            _selectedSeed = seedData;
        }

        private SeedData _selectedSeed;

        protected override void OnExecute()
        {
            var PhaseModel = this.GetModel<IPhaseModel>();
            var HandSystem = this.GetSystem<IHandSystem>();
            var LevelModel = this.GetModel<ILevelModel>();
            // 异常处理
            if (PhaseModel.GamePhase != GamePhase.Gameplay)
                throw new System.Exception($"在不正确的阶段执行了SelectSeedCommand：{PhaseModel.GamePhase}");
            if (HandSystem.HandInfo.Value.HandState != HandState.Empty)
                throw new System.Exception($"执行了SelectSeedCommand，但是手的状态为：{HandSystem.HandInfo.Value.HandState}");
            if (LevelModel.SunPoint.Value < _selectedSeed.CardData.CardDefinition.SunpointCost)
                throw new Exception($"尝试选择植物，但是阳光不充足");
            if (!_selectedSeed.ColdTimeTimer.Ready)
                throw new Exception($"尝试选择植物，但是他还在冷却时间");

            // 
            this.SendEvent<SelectSeedEvent>(new SelectSeedEvent { SelectedSeedData = _selectedSeed });
        }
    }
}