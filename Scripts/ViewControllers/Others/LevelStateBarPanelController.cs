using System;
using DG.Tweening;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others
{
    public class LevelStateBarPanelController : MonoBehaviour, IController
    {
        [SerializeField] private Image Fill;
        [SerializeField] private RectTransform Flags;
        private IWaveSystem _WaveSystem;
        private ILevelModel _LevelModel;

        private const float FlagStartX = -46;
        private const float FlagEndX = -485;
        private const float FlagY = -40;

        public void SpawnFlags()
        {
            var flagPrefab = ResLoader.Allocate().LoadSync<GameObject>(Levelstatebarflag_prefab.BundleName,
                Levelstatebarflag_prefab.LevelStateBarFlag);
            var hugeWaves = _LevelModel.LevelData.HugeWaves;
            foreach (var hugeWave in hugeWaves)
            {
                var rate = (float)hugeWave / _LevelModel.LevelData.TotalWaveCount;
                var flagX = Mathf.Lerp(FlagStartX, FlagEndX, rate);
                var rt = flagPrefab.Instantiate().transform as RectTransform;
                rt.SetParent(Flags,false);
                rt.anchoredPosition = new Vector2(flagX, FlagY);
            }
        }

        private void Awake()
        {
            _WaveSystem = this.GetSystem<IWaveSystem>();
            _LevelModel = this.GetModel<ILevelModel>();

            _WaveSystem.CurrentWave.Register(wave =>
                {
                    var rate = (float)wave / _LevelModel.LevelData.TotalWaveCount;
                    Fill.DOFillAmount(rate, 1f).SetEase(Ease.OutQuad);
                }
            ).UnRegisterWhenGameObjectDestroyed(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}