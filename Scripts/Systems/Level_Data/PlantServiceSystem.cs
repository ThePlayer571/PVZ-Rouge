using QFramework;
using TPL.PVZR.CommandEvents.Level_Gameplay.PlantSpawn;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.Systems.Level_Data
{
    public interface IPlantSpawnSystem : IServiceManageSystem
    {
    }

    public class PlantSpawnSystem : AbstractSystem, IPlantSpawnSystem
    {
        protected override void OnInit()
        {
            this.RegisterEvent<SpawnPlantEvent>(e =>
            {
                var go = EntityFactory.PlantFactory.SpawnPlant(e.Def, e.Direction, e.CellPos);
                this.SendEvent<OnPlantSpawned>(new OnPlantSpawned { CellPos = e.CellPos, Plant = go });
            });
        }
    }
}