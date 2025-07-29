using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public sealed class CoffeeBean : Plant
    {
        public override PlantDef Def { get; } = new PlantDef(PlantId.CoffeeBean, PlantVariant.V0);

        protected override void OnInit()
        {
            HealthPoint = GlobalEntityData.Plant_Default_Health;

            ActionKit.Sequence()
                .Delay(1)
                .Callback(() =>
                {
                    var sleepingShroom = AttachedCell.CellPlantData.GetPlant(PlacementSlot.Normal) as ISleepyShroom;

                    if (sleepingShroom is { IsAwake: false }) sleepingShroom.Awaken();

                    this.Kill();
                }).Start(this);
        }
    }
}