using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Tools;

namespace TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs
{
    public struct SelectSeedEvent
    {
        public SeedData SelectedSeedData;
    }

    public struct DeselectEvent
    {
    }

    public struct SelectShovelEvent
    {
    }

    public struct PlantingSeedInHandEvent
    {
        public Direction2 Direction;
        public SeedData PlantedSeed;
    }

    public struct UseShovelEvent
    {
    }
}