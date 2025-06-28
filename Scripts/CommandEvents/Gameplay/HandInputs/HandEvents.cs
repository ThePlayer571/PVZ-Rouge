using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Core;
using TPL.PVZR.Systems;
using TPL.PVZR.ViewControllers.Others;

namespace TPL.PVZR.Events.HandEvents
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