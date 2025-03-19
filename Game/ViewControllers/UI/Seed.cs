using UnityEngine;
using QFramework;
using Unity.VisualScripting;

namespace TPL.PVZR
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

        // 框架接口
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
        // Model|System
        private ILevelModel _LevelModel;
        private IHandSystem _HandSystem;
        private InputSystem _InputSystem;
        // 数据
        public SeedDataSO seedData;
        public int seedIndex;
        // 变量
        private bool _isSelected;
        private float _coldTimeTimer;
        private ColdState _coldState;
        private SunNeedState _sunNeedState;
        // 属性
        public int sunpointCost => seedData.sunpointCost;
        public bool isSelectable => _coldState == ColdState.Ready && _sunNeedState == SunNeedState.Enough;
        // 初始化

        public void Init(CardDataSO cardData)
        {
            // 设置数据
            seedData = cardData.seedData;
            Plant.sprite = seedData.plantSprite;
            SunText.text = seedData.sunpointCost.ToString();
            // == 接受事件
            // 阳光变化
            _LevelModel.sunpoint.RegisterWithInitValue(val =>
            {
                if (seedData.sunpointCost <= val)
                {
                    _sunNeedState = SunNeedState.Enough;
                }
                else
                {
                    _sunNeedState = SunNeedState.NotEnough;
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            // 初始化数据
            if (seedData.haveInitialColdTime)
            {
                _coldTimeTimer = seedData.coldTime;
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
        // == 逻辑
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
        // 操作
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
            _coldTimeTimer = seedData.coldTime;
            _coldState = ColdState.InCold;
        }
        // 函数
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
                BlackMask.fillAmount = _coldTimeTimer / seedData.coldTime;
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








