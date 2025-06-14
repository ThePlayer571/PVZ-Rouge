using QFramework;

namespace TPL.PVZR.Core.Save
{
    public interface ISaveModel : IModel
    {
        void Save(SaveKey key, object data);
        object Load(SaveKey key);
    }

    public class SaveModel : AbstractModel, ISaveModel
    {
        #region Public

        public void Save(SaveKey key, object data)
        {
            throw new System.NotImplementedException();
        }

        public object Load(SaveKey key)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        protected override void OnInit()
        {
            throw new System.NotImplementedException();
        }
    }
}