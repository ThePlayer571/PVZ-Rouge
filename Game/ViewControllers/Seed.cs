using UnityEngine;
using QFramework;

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
        // 数据
        public CardSO cardSO;
        [SerializeField] public int seedIndex = 1;
        // 变量
        private bool _isSelected;
        private float _coldTimeTimer;
        private ColdState _coldState;
        private SunNeedState _sunNeedState;
        // 属性
        public int sunpointCost => cardSO.sunpointCost;
        public bool isSelectable => _coldState == ColdState.Ready && _sunNeedState == SunNeedState.Enough;
        // 初始化
        private void Awake()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _HandSystem = this.GetSystem<IHandSystem>();
            // 阳光变化
            _LevelModel.sunpoint.RegisterWithInitValue(val =>
            {
                if (cardSO.sunpointCost <= val)
                {
                    _sunNeedState = SunNeedState.Enough;
                }
                else
                {
                    _sunNeedState = SunNeedState.NotEnough;
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            // 

            // 选择卡牌
            this.RegisterEvent<OnSelectSeed>((@event) => {
                if (@event.seed == this)
                {
                    OnSelected();
                } }).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            // 取消选择卡牌
            this.RegisterEvent<OnDeselectSeed>((@event) => {
                if (@event.seed == this)
                {
                    OnDeselected();
                } }).UnRegisterWhenGameObjectDestroyed(gameObject);
            // 放置卡牌
            this.RegisterEvent<OnPlacePlant>((@event) =>
            {
                if (@event.seed == this)
                {
                    OnPlanted();
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            // 初始化数据
            if (cardSO.haveInitialColdTime)
            {
                _coldTimeTimer = cardSO.coldTime;
                _coldState = ColdState.InCold;
            }
            else
            {
                _coldTimeTimer = 0;
                _coldState = ColdState.Ready;
            }
            _isSelected = false;
        }
        private void Start()
        {
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
        private void OnSelected()
        {
            _isSelected = true;
        }
        private void OnDeselected()
        {
            _isSelected = false;
        }

        private void OnPlanted()
        {
            _isSelected = false;
            _coldTimeTimer = cardSO.coldTime;
            _coldState = ColdState.InCold;
        }
        // 函数
        private void UIUpdate()
        {
            if (_isSelected == true)
            {
                Normal.Hide();
                Gray.Show();
                Mask.Hide();
            }
            else if (_coldState == ColdState.InCold)
            {
                Normal.Hide();
                Gray.Show();
                Mask.Show();
                Mask.fillAmount = _coldTimeTimer / cardSO.coldTime;
            }
            else if (_coldState == ColdState.Ready)
            {
                if (_sunNeedState == SunNeedState.Enough)
                {

                    Normal.Show();
                    Gray.Hide();
                    Mask.Hide();
                }
                else if (_sunNeedState == SunNeedState.NotEnough)
                {
                    Normal.Hide();
                    Gray.Show();
                    Mask.Hide();
                }
            }
        }

    }
}








