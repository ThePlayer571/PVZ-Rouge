using System.Collections.Generic;
using System.Threading.Tasks;
using TPL.PVZR.Classes.Others;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.DataReader
{
    public static class TipConfigReader
    {
        #region 数据存储

        private static Dictionary<TipId, TipDefinition> _tipDict;

        public static async Task InitializeAsync()
        {
            var handle = Addressables.LoadAssetsAsync<TipDefinition>("TipDefinition", null);
            await handle.Task;
            _tipDict = new Dictionary<TipId, TipDefinition>();
            foreach (var tip in handle.Result)
            {
                _tipDict.Add(tip.Id, tip);
            }
        }

        #endregion
        
        public static TipDefinition GetTipDefinition(TipId id)
        {
            
            if (_tipDict.TryGetValue(id, out var definition))
            {
                return definition;
            }

            throw new KeyNotFoundException($"未找到Tip: {id}");
            
        }
    }
}