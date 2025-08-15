using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.Others;
using TPL.PVZR.Helpers.New.DataReader;

namespace TPL.PVZR.Services
{
    public interface ITipService : IService
    {
        void AddTip(TipId tipId);
        bool HasTip();
        TipDefinition PopTip();
    }

    public class TipService : AbstractService, ITipService
    {
        private Queue<TipId> _tipQueue = new();

        protected override void OnInit()
        {
        }

        public void AddTip(TipId tipId)
        {
            _tipQueue.Enqueue(tipId);
        }

        public bool HasTip()
        {
            return _tipQueue.Count > 0;
        }

        public TipDefinition PopTip()
        {
            var id = _tipQueue.Dequeue();
            return TipConfigReader.GetTipDefinition(id);
        }
    }
}