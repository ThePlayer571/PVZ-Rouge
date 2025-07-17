using TPL.PVZR.Classes.DataClasses_InLevel.ZombieArmor;
using TPL.PVZR.Helpers.New.DataReader;

namespace TPL.PVZR.Helpers.New.ClassCreator
{
    public static class ZombieArmorCreator
    {
        public static ZombieArmorData CreateZombieArmorData(ZombieArmorId zombieArmorId)
        {
            var definition = ZombieArmorConfigReader.GetZombieArmorDefinition(zombieArmorId);
            return new ZombieArmorData(definition);
        }
    }
}