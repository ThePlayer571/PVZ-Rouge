using UnityEngine;
using QFramework;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace TPL.PVZR
{
    public partial class Card : ViewController,IController
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
        private IGameModel _GameModel;
        private IHandSystem _HandSystem;
        // 数据
        public CardData cardData;
        [SerializeField] public int cardIndex = 1;
        // 变量
        private bool _isSelected;
        private float _coldTimeTimer;
        private ColdState _coldState;
        private SunNeedState _sunNeedState;
        // 属性
        public int sunpointCost => cardData.sunpointCost;
        public bool isSelectable => _coldState == ColdState.Ready && _sunNeedState == SunNeedState.Enough;
        // 初始化
        private void Awake()
        {
            _GameModel = this.GetModel<IGameModel>();
            _HandSystem = this.GetSystem<IHandSystem>();
            // 阳光变化
            _GameModel.sunpoint.RegisterWithInitValue(val =>
            {
                if (cardData.sunpointCost <= val)
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
            this.RegisterEvent<OnSelectCard>((@event) => {
                if (@event.card == this)
                {
                    OnSelected();
                } }).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            // 取消选择卡牌
            this.RegisterEvent<OnDeselectCard>((@event) => {
                if (@event.card == this)
                {
                    OnDeselected();
                } }).UnRegisterWhenGameObjectDestroyed(gameObject);
            // 放置卡牌
            this.RegisterEvent<OnPlacePlant>((@event) =>
            {
                if (@event.card == this)
                {
                    OnPlanted();
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            // 初始化数据
            if (cardData.haveInitialColdTime)
            {
                _coldTimeTimer = cardData.coldTime;
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
            _coldTimeTimer = cardData.coldTime;
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
                Mask.fillAmount = _coldTimeTimer / cardData.coldTime;
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








