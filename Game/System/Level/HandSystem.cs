using QFramework;
using TPL.PVZR.EntityPlant;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static TPL.PVZR.HandSystem;

namespace TPL.PVZR
{
    public interface IHandSystem : ISystem, IInLevelSystem
    {
        HandState handState { get; }
        bool handIsOnUI { get; }
        Vector3 handWorldPos { get; }
        void TrySelect(Seed seed);
    }
    public class HandSystem : AbstractSystem, IHandSystem
    {
        public enum HandState
        {
            Empty, HavePlant, HaveShovel
        }
        // Model
        private ILevelModel _LevelModel;
        private IEntityCreateSystem _EntityCreateSystem;
        private LevelSystem _LevelSystem;
        // 节点
        private GameObject SelectFramebox;
        private GameObject FollowingSprite;
        private Image ShovelImage;
        // 变量
        private Seed _selectedSeed = null;
        // 属性
        public HandState handState { get; private set; } = HandState.Empty; 
        public Vector3 handWorldPos => Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        private Vector3Int handCellPos => _LevelModel.Grid.WorldToCell(handWorldPos);
        private Vector2Int handCellPos2 => new Vector2Int(handCellPos.x, handCellPos.y); // 有些时候计算需要用到vector3 所以有handCellPos 2D 3D

        private Cell HandOnCell => _LevelModel.CellGrid[handCellPos2.x, handCellPos2.y];

        private Cell HandDownCell => _LevelModel.CellGrid[handCellPos2.x, handCellPos2.y-1];

        private bool daveHandCanReachMousePos =>
            Vector2.Distance(_LevelModel.Dave.transform.position, handWorldPos) < 5f;

        private bool selectedPlantCanPlaceOnMousePos  {
            get
            {
                if (!_selectedSeed) return false;
                //
                if (_selectedSeed.seedData.plantIdentifier is PlantIdentifier.Flowerpot) // 花盆
                {
                    return HandOnCell.CanPlantHere && HandDownCell.CanPotAbove && daveHandCanReachMousePos;
                }
                else // 其他植物
                {
                    return HandOnCell.CanPlantHere && HandDownCell.CanPlantAbove && daveHandCanReachMousePos;
                }
            }
        }

        public bool handIsOnUI => EventSystem.current.IsPointerOverGameObject();
        // 初始化
        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _EntityCreateSystem = this.GetSystem<IEntityCreateSystem>();
            _LevelSystem = this.GetSystem<LevelSystem>();
            //
            this.RegisterEvent<EnterGameSceneInitEvent>(@event => OnEnterGameSceneInit());
            //
            this.RegisterEvent<InputDeselectEvent>(@event => TryDeselect());
            this.RegisterEvent<InputPlacePlantEvent>(@event => TryPlacePlant(@event.direction));
            this.RegisterEvent<InputSelectEvent>(@event => TrySelect(_LevelModel.GetSeed(@event.seedIndex)));
            this.RegisterEvent<InputSelectForceEvent>(@event =>
            {
                TryDeselect();
                TrySelect(@_LevelModel.GetSeed(@event.seedIndex));
            });
            this.RegisterEvent<InputPickShovelEvent>(@event => TryPickShovel());
            this.RegisterEvent<InputUseShovelEvent>(@event => TryUseShovel());
            //
        }


        public void OnBuildingLevel()
        {
            ResLoader _ResLoader = ResLoader.Allocate();
            FollowingSprite = _ResLoader.LoadSync<GameObject>("FollowingSprite").Instantiate();
            FollowingSprite.GetComponent<SpriteRenderer>().sortingLayerName = "HandItem";
            FollowingSprite.Hide();
            SelectFramebox = _ResLoader.LoadSync<GameObject>("SelectFramebox").Instantiate();
            SelectFramebox.GetComponent<SpriteRenderer>().sortingLayerName = "SelectFramebox";
            SelectFramebox.Hide();
            ActionKit.DelayFrame(1, () => GameManager.ExecuteOnUpdate(Update)).Start(GameManager.Instance);
        }

        public void OnGameplay()
        {
            ShovelImage = GameObject.Find("ShovelImage").GetComponent<Image>();
        }

        public void OnEnd()
        {
            FollowingSprite = null;
            SelectFramebox = null;
            ShovelImage = null;
        }
        void OnEnterGameSceneInit()
        {
           }
        // == 逻辑
        private void Update()
        {
            if (handState is  HandState.HavePlant or HandState.HaveShovel)
            {
                FollowingSprite.Position2D(handWorldPos);
            }
            // 跟随图片
            if (_LevelSystem.levelState.CurrentStateId == LevelSystem.LevelState.Gameplay)
            {
                if (handState is HandState.HavePlant)
                {
                    if (selectedPlantCanPlaceOnMousePos)
                    {
                        SelectFramebox.Show();
                        SelectFramebox.Position2D(_LevelModel.Grid.CellToWorld(handCellPos) +
                                                  _LevelModel.Grid.cellSize * 0.5f);
                    }
                    else
                    {
                        SelectFramebox.Hide();
                    }
                }
                else if ( handState is HandState.Empty)
                {
                    if (HandOnCell.CanPlantHere && HandDownCell.CanPotAbove && daveHandCanReachMousePos)
                    {
                        SelectFramebox.Show();
                        SelectFramebox.Position2D(_LevelModel.Grid.CellToWorld(handCellPos) +
                                                  _LevelModel.Grid.cellSize * 0.5f);
                    }
                    else
                    {
                        SelectFramebox.Hide();
                    }
                }
                else if (handState is HandState.HaveShovel)
                {
                    if (HandOnCell.HavePlant)
                    {
                        SelectFramebox.Show();
                        SelectFramebox.Position2D(_LevelModel.Grid.CellToWorld(handCellPos) +
                                                  _LevelModel.Grid.cellSize * 0.5f);
                    }
                    else
                    {
                        SelectFramebox.Hide();

                    }
                }
            }
        }
        // 操作
        public void TrySelect(Seed seed)
        {
            if (seed.isSelectable && handState == HandState.Empty)
            {
                Select(seed);
            }
        }
        private void Select(Seed seed)
        {
            //
            _selectedSeed = seed;
            handState = HandState.HavePlant;
            //
            FollowingSprite.GetComponent<SpriteRenderer>().sprite = seed.seedData.followingSprite;
            FollowingSprite.Show();
            //
            seed.OnSelected();
        }

        public void TryPickShovel()
        {
            if (handState == HandState.Empty)
            {
                PickShovel();
            }
        }
        private void PickShovel()
        {
            handState = HandState.HaveShovel;
            //
            FollowingSprite.GetComponent<SpriteRenderer>().sprite = ShovelImage.sprite;
            FollowingSprite.Show();
            ShovelImage.Hide();
        }
        private void TryDeselect()
        {
            if (handState is HandState.HavePlant or HandState.HaveShovel)
            {
                Deselect();
            }
        }
        private void Deselect()
        {
            if (handState == HandState.HavePlant)
            {
                _selectedSeed.OnDeselected();
                //
                _selectedSeed = null;
                handState = HandState.Empty;
                //
                FollowingSprite.Hide();
            } else if (handState == HandState.HaveShovel)
            {
                
                handState = HandState.Empty;
                //
                FollowingSprite.Hide();
                ShovelImage.Show();
            }

        }
        private void TryPlacePlant(Direction2 direction)
        {
            if (selectedPlantCanPlaceOnMousePos)
            {
                PlacePlant(direction);
            }
        }

        private void PlacePlant(Direction2 direction)
        {
            _selectedSeed.OnPlanted();
            // 新建植物对象
            GameObject go = _EntityCreateSystem.CreatePlant(_selectedSeed.seedData.plantIdentifier, handCellPos2, direction);
            // 阳光
            _LevelModel.sunpoint.Value -= _selectedSeed.sunpointCost;
            // 处理手持卡牌
            _selectedSeed = null;
            handState = HandState.Empty;
            // 图片跟随
            FollowingSprite.Hide();
        }

        private void TryUseShovel()
        {
            if (handState == HandState.HaveShovel &&
                HandOnCell.HavePlant)
            {
                UseShovel();
            }
        }

        private void UseShovel()
        {
            HandOnCell.plant.Kill();
            handState = HandState.Empty;
            FollowingSprite.Hide();
            ShovelImage.Show();
        }
    }
}