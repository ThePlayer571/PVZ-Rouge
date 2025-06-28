using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Tools;

namespace TPL.PVZR.CommandEvents.Gameplay.HandInputs
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