using QFramework;

namespace TPL.PVZR.Systems
{
    public interface IWaveSystem : ISystem
    {
        
    }
    
    public class WaveSystem : AbstractSystem, IWaveSystem
    {
        protected override void OnInit()
        {
            throw new System.NotImplementedException();
        }
    }
}