using QFramework;
using TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs;
using TPL.PVZR.Models;

namespace TPL.PVZR.Systems.Level_Data
{
    public interface ISunSystem : ISystem
    {
    }

    public class SunSystem : AbstractSystem, ISunSystem
    {
        private ILevelModel _LevelModel;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();

            this.RegisterEvent<OnSeedInHandPlanted>(e =>
            {
                _LevelModel.SunPoint.Value -= e.PlantedSeed.CardData.CardDefinition.SunpointCost;
            });
        }
    }
}