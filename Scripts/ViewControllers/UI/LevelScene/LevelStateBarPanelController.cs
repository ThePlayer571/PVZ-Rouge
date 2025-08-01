using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI
{
    public class LevelStateBarPanelController : MonoBehaviour, IController
    {
        [SerializeField] private Image Fill;
        [SerializeField] private RectTransform Flags;
        private ILevelModel _LevelModel;
        private AsyncOperationHandle<GameObject> _flagsHandle;

        private const float FlagStartX = -46;
        private const float FlagEndX = -485;
        private const float FlagStartY = -40;
        private const float FlagEndY = 0;

        private Dictionary<int, RectTransform> _flagsDict;

        public async Task SpawnFlags()
        {
            if (!_flagsHandle.IsValid() || _flagsHandle.Status != AsyncOperationStatus.Succeeded)
            {
                _flagsHandle = Addressables.LoadAssetAsync<GameObject>("LevelStateBarFlag");
                await _flagsHandle.Task;
            }
            
            var flagPrefab = _flagsHandle.Result;
            var hugeWaves = _LevelModel.LevelData.HugeWaves;
            foreach (var hugeWave in hugeWaves)
            {
                var rate = (float)hugeWave / _LevelModel.LevelData.TotalWaveCount;
                var flagX = Mathf.Lerp(FlagStartX, FlagEndX, rate);
                var rt = flagPrefab.Instantiate().transform as RectTransform;
                rt.SetParent(Flags, false);
                rt.anchoredPosition = new Vector2(flagX, FlagStartY);
                _flagsDict.Add(hugeWave, rt);
            }
        }

        private void Awake()
        {
            _LevelModel = this.GetModel<ILevelModel>();

            _flagsDict = new Dictionary<int, RectTransform>();

            _LevelModel.CurrentWave.Register(wave =>
                {
                    // 进度条
                    var rate = (float)wave / _LevelModel.LevelData.TotalWaveCount;
                    Fill.DOFillAmount(rate, 1f).SetEase(Ease.OutQuad);
                    // 旗子
                    if (_LevelModel.LevelData.HugeWaves.Contains(wave))
                    {
                        var flag = _flagsDict[wave];
                        flag.DOAnchorPosY(FlagEndY, 1f).SetEase(Ease.InOutQuad);
                    }
                }
            ).UnRegisterWhenGameObjectDestroyed(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}