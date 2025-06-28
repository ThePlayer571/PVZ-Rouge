using QFramework;
using TPL.PVZR.Classes.MazeMap.Interfaces;
using TPL.PVZR.Models;

namespace TPL.PVZR.Classes.MazeMap.Public.Bases
{
    public abstract class MazeMapController : IMazeMapController
    {
        #region Public

        public IMazeMapData MazeMapData { get; protected set; }

        public abstract void SetMazeMapTiles();
        
        #endregion

        
        
        
        protected IPhaseModel _PhaseModel;
        
        protected MazeMapController(IMazeMapData mazeMapData)
        {
            _PhaseModel = this.GetModel<IPhaseModel>();

            this.MazeMapData = mazeMapData;
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}