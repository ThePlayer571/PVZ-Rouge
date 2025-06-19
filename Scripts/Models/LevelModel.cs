using QFramework;
using TPL.PVZR.Classes.Level;

namespace TPL.PVZR.Models
{
    public interface ILevelModel : IModel
    {
        // Data
        int SunPoint { get; set; }
        
        // Methods
        void Init(ILevelData levelData);

    }


    public class LevelModel : AbstractModel,ILevelModel
    {
        public int SunPoint { get; set; }
        public void Init(ILevelData levelData)
        {
            this.SunPoint = levelData.InitialSunPoint;
        }


        protected override void OnInit()
        {
            // do nothing
        }

    }
}