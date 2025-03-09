using QFramework;
using TPL.PVZR.EntityPlant;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static TPL.PVZR.HandSystem;

namespace TPL.PVZR
{
    public interface IHandSystem : ISystem
    {
        HandState handState { get; }
        bool handIsOnUI { get; }
        Vector3 handWorldPos { get; }
        void TrySelect(Card card);
    }
    public class HandSystem : AbstractSystem, IHandSystem
    {
        public enum HandState
        {
            Empty, HavePlant, HaveShovel
        }
        // Model
        private IGameModel _GameModel;
        private IEntityCreateSystem _EntityCreateSystem;
        // 节点
        private GameObject SelectFramebox;
        private GameObject FollowingSprite;
        // 变量
        [SerializeField]
        private HandState _handState = HandState.Empty;
        private Card _selectedCard = null;
        // 属性
        public HandState handState => _handState;
        public Vector3 handWorldPos => Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        private Vector3Int handCellPos => _GameModel.Grid.WorldToCell(handWorldPos);
        private Vector2Int handCellPos2 => new Vector2Int(handCellPos.x, handCellPos.y); // 有些时候计算需要用到vector3 所以有handCellPos 2D 3D

        private Cell handOnCell => _GameModel.CellGrid[handCellPos2.x, handCellPos2.y];

        private Cell handDownCell => _GameModel.CellGrid[handCellPos2.x, handCellPos2.y-1];

        public bool handIsOnUI => EventSystem.current.IsPointerOverGameObject();
        // 初始化
        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();
            _EntityCreateSystem = this.GetSystem<IEntityCreateSystem>();
            //
            this.RegisterEvent<EnterGameSceneInitEvent>(@event => OnEnterGameSceneInit());
            //
            this.RegisterEvent<InputDeselectEvent>(@event => TryDeselect());
            this.RegisterEvent<InputPlacePlantEvent>(@event => TryPlacePlant(@event.direction));
            this.RegisterEvent<InputSelectEvent>(@event => TrySelect(@event.card));
            this.RegisterEvent<InputSelectForceEvent>(@event =>
            {
                TryDeselect();
                TrySelect(@event.card);
            });
            this.RegisterEvent<InputPickShovelEvent>(@event => TryPickShovel());
            this.RegisterEvent<InputUseShovelEvent>(@event => TryUseShovel());
            //
        }

        void OnEnterGameSceneInit()
        {
            ResLoader _ResLoader = ResLoader.Allocate();
            FollowingSprite = _ResLoader.LoadSync<GameObject>("FollowingSprite").Instantiate();
            SelectFramebox = _ResLoader.LoadSync<GameObject>("SelectFramebox").Instantiate();
            FollowingSprite.Hide();
            ActionKit.DelayFrame(1, () => GameManager.ExecuteOnUpdate(Update)).Start(GameManager.Instance);
        }
        // == 逻辑
        private void Update()
        {
            if (_handState is  HandState.HavePlant or HandState.HaveShovel)
            {
                FollowingSprite.Position2D(handWorldPos);
            }
            SelectFramebox.Position2D(_GameModel.Grid.CellToWorld(handCellPos) + _GameModel.Grid.cellSize * 0.5f);
        }
        // 操作
        public void TrySelect(Card card)
        {
            if (card.isSelectable && _handState == HandState.Empty)
            {
                Select(card);
            }
        }
        private void Select(Card card)
        {
            //
            _selectedCard = card;
            _handState = HandState.HavePlant;
            //
            FollowingSprite.GetComponent<SpriteRenderer>().sprite = card.cardData.followingSprite;
            FollowingSprite.Show();
            //
            this.SendEvent<OnSelectCard>(new() { card = card });
        }

        public void TryPickShovel()
        {
            if (_handState == HandState.Empty)
            {
                PickShovel();
            }
        }
        private void PickShovel()
        {
            _handState = HandState.HaveShovel;
            
            //
            FollowingSprite.GetComponent<SpriteRenderer>().sprite = _GameModel.shovel.GetComponent<Image>().sprite;
            FollowingSprite.Show();
        }
        private void TryDeselect()
        {
            if (_handState is HandState.HavePlant or HandState.HaveShovel)
            {
                Deselect();
            }
        }
        private void Deselect()
        {
            if (_handState == HandState.HavePlant)
            {
                Card _temp_selectedCard = _selectedCard;
                //
                _selectedCard = null;
                _handState = HandState.Empty;
                //
                FollowingSprite.Hide();
                //
                this.SendEvent<OnDeselectCard>(new() { card = _temp_selectedCard });
            } else if (_handState == HandState.HaveShovel)
            {
                
                _handState = HandState.Empty;
                //
                FollowingSprite.Hide();
            }

        }
        private void TryPlacePlant(Direction2 direction)
        {
            if (_handState == HandState.HavePlant && handOnCell.IsEmpty && handDownCell.CanPlantOn)
            {
                PlacePlant(direction);
            }
        }

        private void PlacePlant(Direction2 direction)
        {
            Card _temp_selectedCard = _selectedCard;
            // 新建植物对象
            GameObject go = _EntityCreateSystem.CreatePlant(_selectedCard.cardData.plantIdentifier, handCellPos2, direction);
            // 处理手持卡牌
            _selectedCard = null;
            _handState = HandState.Empty;
            // 图片跟随
            FollowingSprite.Hide();
            // 阳光
            _GameModel.sunpoint.Value -= _temp_selectedCard.sunpointCost;
            // 事件
            this.SendEvent<OnPlacePlant>(new OnPlacePlant { card = _temp_selectedCard , plant = go.GetComponent<PeaShooter>() });
        }

        private void TryUseShovel()
        {
            if (_handState == HandState.HaveShovel &&
                handOnCell.HavePlant)
            {
                UseShovel();
            }
        }

        private void UseShovel()
        {
            handOnCell.plant.Kill();
            _handState = HandState.Empty;
            FollowingSprite.Hide();
        }

    }
}