using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems;
using TPL.PVZR.Architecture.Systems.InLevel;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Gameplay.ViewControllers.UI
{
    public partial class Seed : ViewController,IController
    {
        private enum ColdState
        {
            InCold, Ready
        }

        private enum SunNeedState
        {
            Enough, NotEnough
        }

        // ��ܽӿ�
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
        // Model|System
        private ILevelModel _LevelModel;
        private IHandSystem _HandSystem;
        private InputSystem _InputSystem;
        // ����
        [FormerlySerializedAs("seedData")] public SeedSO seedSO;
        public int seedIndex;
        // ����
        private bool _isSelected;
        private float _coldTimeTimer;
        private ColdState _coldState;
        private SunNeedState _sunNeedState;
        // ����
        public int sunpointCost => seedSO.sunpointCost;
        public bool isSelectable => _coldState == ColdState.Ready && _sunNeedState == SunNeedState.Enough;
        // ��ʼ��

        public void Init(SeedSO seedSO)
        {
            // ��������
            this.seedSO = seedSO;
            Plant.sprite = seedSO.plantSprite;
            SunText.text = seedSO.sunpointCost.ToString();
            // == �����¼�
            // ����仯
            _LevelModel.sunpoint.RegisterWithInitValue(val =>
            {
                if (seedSO.sunpointCost <= val)
                {
                    _sunNeedState = SunNeedState.Enough;
                }
                else
                {
                    _sunNeedState = SunNeedState.NotEnough;
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            // ��ʼ������
            if (seedSO.haveInitialColdTime)
            {
                _coldTimeTimer = seedSO.coldTime;
                _coldState = ColdState.InCold;
            }
            else
            {
                _coldTimeTimer = 0;
                _coldState = ColdState.Ready;
            }
            _isSelected = false;
            
        }
        private void Awake()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _HandSystem = this.GetSystem<IHandSystem>();
            _InputSystem = this.GetSystem<InputSystem>();
            Btn.onClick.AddListener(() =>
            {
                _InputSystem.TriggerOnSeedButtonClick(this);
            });
        }
        // == �߼�
        private void Update()
        {
            if (_coldState == ColdState.InCold)
            {
                _coldTimeTimer -= Time.deltaTime;
                if (_coldTimeTimer < 0)
                {
                    _coldTimeTimer = 0;
                    _coldState = ColdState.Ready;
                }
            }
            UIUpdate();
        }
        // ����
        public void OnSelected()
        {
            _isSelected = true;
        }
        public void OnDeselected()
        {
            _isSelected = false;
        }

        public void OnPlanted()
        {
            _isSelected = false;
            _coldTimeTimer = seedSO.coldTime;
            _coldState = ColdState.InCold;
        }
        // ����
        private void UIUpdate()
        {
            if (_isSelected)
            {
                Plant.Hide();
                GrayMask.Show();
                BlackMask.Hide();
            }
            else if (_coldState == ColdState.InCold)
            {
                Plant.Show();
                GrayMask.Show();
                BlackMask.Show();
                BlackMask.fillAmount = _coldTimeTimer / seedSO.coldTime;
            }
            else if (_coldState == ColdState.Ready)
            {
                if (_sunNeedState == SunNeedState.Enough)
                {
                    Plant.Show();
                    GrayMask.Hide();
                    BlackMask.Hide();
                }
                else if (_sunNeedState == SunNeedState.NotEnough)
                {
                    Plant.Show();
                    GrayMask.Show();
                    BlackMask.Hide();
                }
            }
        }

    }
}








