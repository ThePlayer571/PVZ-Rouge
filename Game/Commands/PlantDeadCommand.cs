using QFramework;
using TPL.PVZR.EntityPlant;

namespace TPL.PVZR
{
    public class PlantDeadCommand: AbstractCommand
    {
        private Plant plant;
        public PlantDeadCommand(Plant plant)
        {
            
        }
        protected override void OnExecute()
        {
            IGameModel _GameModel = this.GetModel<IGameModel>();

        }
    }
}