using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Tools;

namespace TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs
{
    public struct OnSeedInHandPlanted
    {
        public Direction2 Direction;
        public SeedData PlantedSeed;
    }
}