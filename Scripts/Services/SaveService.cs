using QFramework;
using TPL.PVZR.Tools.Save;

namespace TPL.PVZR.Services
{
    public interface ISaveService : IService
    {
        SaveManager SaveManager { get; }
    }
    
    public class SaveService : AbstractService, ISaveService
    {
        private SaveManager _saveManager;
        
        protected override void OnInit()
        {
            _saveManager = new SaveManager();
        }

        public SaveManager SaveManager => _saveManager;
    }
}