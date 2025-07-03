using QFramework;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs;
using TPL.PVZR.Models;

namespace TPL.PVZR.Systems
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

            this.RegisterEvent<PlantingSeedInHandEvent>(e =>
            {
                _LevelModel.SunPoint.Value -= e.PlantedSeed.CardData.CardDefinition.SunpointCost;
            });
        }
    }
}