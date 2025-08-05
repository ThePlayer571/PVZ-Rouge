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
                var task_1 = EntityFactory.CoinFactory.InitializeAsync();
                var task_2 = EntityFactory.SunFactory.InitializeAsync();
                var task_3 = ShitFactory.InitializeAsync();
                var task_4 = AttackConfigReader.InitializeAsync();
                var task_5 = ZombieArmorConfigReader.InitializeAsync();
                var task_6 = GameConfigReader.InitializeAsync();

                phaseService.AddAwait(Task.WhenAll(task_1, task_2, task_3, task_4, task_5, task_6));
            });
        }
    }
}