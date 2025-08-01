using QFramework;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Helpers.New.GameObjectFactory;

namespace TPL.PVZR.Systems
{
    public interface IConfigLoadSystem : ISystem
    {
        void LoadConfig();
    }

    public class ConfigLoadSystem : AbstractSystem, IConfigLoadSystem
    {
        protected override void OnInit()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            EntityFactory.CoinFactory.InitializeAsync();
            EntityFactory.SunFactory.InitializeAsync();
            ShitFactory.InitializeAsync();

            AttackConfigReader.Initialize();
            ZombieArmorConfigReader.Initialize();
        }
    }
}