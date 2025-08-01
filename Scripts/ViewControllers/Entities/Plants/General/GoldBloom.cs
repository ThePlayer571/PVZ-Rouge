using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents.Level_Gameplay;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class GoldBloom : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.GoldBloom, PlantVariant.V0);

        protected override void OnInit()
        {
            this.HealthPoint = GlobalEntityData.Plant_Default_Health;

            ActionKit.Sequence()
                .Delay(0.5f)
                .Callback(() =>
                {
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                })
                .Delay(0.5f)
                .Callback(() =>
                {
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                })
                .Delay(0.5f)
                .Callback(() =>
                {
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                    this.SendCommand<SpawnSunFromPlantCommand>(new SpawnSunFromPlantCommand(this, SunId.Sun));
                })
                .Delay(0.5f)
                .Callback(Kill)
                .Start(this);
        }

        public override AttackData TakeAttack(AttackData attackData)
        {
            return null;
        }
    }
}