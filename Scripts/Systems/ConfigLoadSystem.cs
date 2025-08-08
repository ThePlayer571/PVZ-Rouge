using System.Threading.Tasks;
using QFramework;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Services;

namespace TPL.PVZR.Systems
{
    public interface IConfigLoadSystem : ISystem
    {
    }

    public class ConfigLoadSystem : AbstractSystem, IConfigLoadSystem
    {
        protected override void OnInit()
        {
            var phaseService = this.GetService<IPhaseService>();
            phaseService.RegisterCallBack((GamePhase.PreInitialization, PhaseStage.EnterEarly), e =>
            {
                var task_1 = CoinFactory.InitializeAsync();
                var task_2 = SunFactory.InitializeAsync();
                var task_3 = ShitFactory.InitializeAsync();
                var task_4 = AttackConfigReader.InitializeAsync();
                var task_5 = ZombieArmorConfigReader.InitializeAsync();
                var task_6 = GameConfigReader.InitializeAsync();
                var task_7 = RecipeConfigReader.InitializeAsync();
                var task_8 = EconomyConfigReader.InitializeAsync();
                var task_9 = LootPoolConfigReader.InitializeAsync();
                var task_10 = PlantConfigReader.InitializeAsync();
                var task_11 = ZombieConfigReader.InitializeAsync();
                var task_12 = PlantBookConfigReader.InitializeAsync();
                var task_13 = ItemViewFactory.InitializeAsync();

                phaseService.AddAwait(Task.WhenAll(task_1, task_2, task_3, task_4, task_5, task_6, task_7, task_8,
                    task_9, task_10, task_11, task_12, task_13));
            });
        }
    }
}