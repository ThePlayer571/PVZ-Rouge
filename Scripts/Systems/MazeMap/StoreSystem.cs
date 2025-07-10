using QFramework;

namespace TPL.PVZR.Systems
{
    public interface IStoreSystem : ISystem
    {
        
    }
    
    public class StoreSystem : AbstractSystem, IStoreSystem
    {
        protected override void OnInit()
        {
            // this.RegisterEvent<>();
        }
    }
}