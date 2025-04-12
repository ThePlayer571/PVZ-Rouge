using QAssetBundle;
using QFramework;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Events.Input;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Entities;
using TPL.PVZR.Gameplay.ViewControllers.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TPL.PVZR.Architecture.Systems.InLevel
{
    public interface IHandSystem : ISystem
    {
        HandSystem.HandState currentHandState { get; }

        // TODO: ↓史山，不过没大问题
        bool isHandOnUI { get; }
    }

    public partial class HandSystem : AbstractSystem, IHandSystem
    {
        public enum HandState
        {
            Empty,
            HavePlant,
            HaveShovel
        }

        #region IHandSystem

        public HandState currentHandState { get; private set; } = HandState.Empty;
        public bool isHandOnUI => EventSystem.current.IsPointerOverGameObject();

        #endregion

        #region 一层具象

        public void TrySelect(Seed seed)
        {
            if (seed.isSelectable && currentHandState == HandState.Empty)
            {
                Select(seed);
            }
        }

        private void Select(Seed seed)
        {
            //
            _selectedSeed = seed;
            currentHandState = HandState.HavePlant;
            //
            ReferenceModel.Get.FollowingSprite.GetComponent<SpriteRenderer>().sprite = seed.seedSO.followingSprite;
            ReferenceModel.Get.FollowingSprite.Show();
            //
            seed.OnSelected();
        }

        public void TryPickShovel()
        {
            if (currentHandState == HandState.Empty)
            {
                PickShovel();
            }
        }

        private void PickShovel()
        {
            currentHandState = HandState.HaveShovel;
            //
            ReferenceModel.Get.FollowingSprite.GetComponent<SpriteRenderer>().sprite = ReferenceModel.Get.ShovelImage.sprite;
            ReferenceModel.Get.FollowingSprite.Show();
            ReferenceModel.Get.ShovelImage.Hide();
        }

        private void TryDeselect()
        {
            if (currentHandState is HandState.HavePlant or HandState.HaveShovel)
            {
                Deselect();
            }
        }

        private void Deselect()
        {
            if (currentHandState == HandState.HavePlant)
            {
                _selectedSeed.OnDeselected();
                //
                _selectedSeed = null;
                currentHandState = HandState.Empty;
                //
                ReferenceModel.Get.FollowingSprite.Hide();
            }
            else if (currentHandState == HandState.HaveShovel)
            {
                currentHandState = HandState.Empty;
                //
                ReferenceModel.Get.FollowingSprite.Hide();
                ReferenceModel.Get.ShovelImage.Show();
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
            _EntitySystem.CreatePlant(_selectedSeed.seedSO.plantIdentifier, handCellPos2, direction);
            // 阳光
            _LevelModel.sunpoint.Value -= _selectedSeed.sunpointCost;
            // 处理手持卡牌
            _selectedSeed = null;
            currentHandState = HandState.Empty;
            // 图片跟随
            ReferenceModel.Get.FollowingSprite.Hide();
        }

        private void TryUseShovel()
        {
            if (currentHandState == HandState.HaveShovel &&
                HandOnCell.HavePlant)
            {
                UseShovel();
            }
        }

        private void UseShovel()
        {
            HandOnCell.plant.Kill();
            currentHandState = HandState.Empty;
            ReferenceModel.Get.FollowingSprite.Hide();
            ReferenceModel.Get.ShovelImage.Show();
        }

        #endregion

        // Model|System
        private ILevelModel _LevelModel;
        private IEntitySystem _EntitySystem;
        private IGamePhaseSystem _GamePhaseSystem;

        private ResLoader _ResLoader;

        // 变量
        private Seed _selectedSeed = null;

        #region 属性 用于提高可读性的

        private Vector3 handWorldPos => Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        private Vector3Int handCellPos => ReferenceModel.Get.Grid.WorldToCell(handWorldPos);

        private Vector2Int handCellPos2 =>
            new Vector2Int(handCellPos.x, handCellPos.y); // 有些时候计算需要用到vector3 所以有handCellPos 2D 3D

        private Cell HandOnCell => _LevelModel.CellGrid[handCellPos2.x, handCellPos2.y];

        private Cell HandDownCell => _LevelModel.CellGrid[handCellPos2.x, handCellPos2.y - 1];

        private bool daveHandCanReachMousePos =>
            Vector2.Distance(ReferenceModel.Get.Dave.transform.position, handWorldPos) < 5f;

        private bool selectedPlantCanPlaceOnMousePos
        {
            get
            {
                if (!_selectedSeed) return false;
                //
                if (_selectedSeed.seedSO.plantIdentifier is PlantIdentifier.Flowerpot) // 花盆
                {
                    return HandOnCell.CanPlantHere && HandDownCell.CanPotAbove && daveHandCanReachMousePos;
                }
                else // 其他植物
                {
                    return HandOnCell.CanPlantHere && HandDownCell.CanPlantAbove && daveHandCanReachMousePos;
                }
            }
        }

        #endregion

        // 初始化
        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _EntitySystem = this.GetSystem<IEntitySystem>();
            _GamePhaseSystem = this.GetSystem<IGamePhaseSystem>();
            _ResLoader = ResLoader.Allocate();
            // 
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            // 输入事件
            this.RegisterEvent<InputDeselectEvent>(@event => TryDeselect());
            this.RegisterEvent<InputPlacePlantEvent>(@event => TryPlacePlant(@event.direction));
            this.RegisterEvent<InputSelectEvent>(@event => TrySelect(ReferenceModel.Get.GetSeed(@event.seedIndex)));
            this.RegisterEvent<InputSelectForceEvent>(@event =>
            {
                TryDeselect();
                TrySelect(ReferenceModel.Get.GetSeed(@event.seedIndex));
            });
            this.RegisterEvent<InputPickShovelEvent>(@event => TryPickShovel());
            this.RegisterEvent<InputUseShovelEvent>(@event => TryUseShovel());
            //  PhaseEvents
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelInitialization)
                {
                    ActionKit.DelayFrame(1, () => GameManager.ExecuteOnUpdate(Update)).Start(GameManager.Instance);
                }
            });
        }

        private void Update()
        {
            if (currentHandState is HandState.HavePlant or HandState.HaveShovel)
            {
                ReferenceModel.Get.FollowingSprite.Position2D(handWorldPos);
            }

            // 跟随图片
            if (_GamePhaseSystem.currentGamePhase is GamePhaseSystem.GamePhase.Gameplay)
            {
                if (currentHandState is HandState.HavePlant)
                {
                    if (selectedPlantCanPlaceOnMousePos)
                    {
                        ReferenceModel.Get.SelectFramebox.Show();
                        ReferenceModel.Get.SelectFramebox.Position2D(ReferenceModel.Get.Grid.CellToWorld(handCellPos) +
                                                                     ReferenceModel.Get.Grid.cellSize * 0.5f);
                    }
                    else
                    {
                        ReferenceModel.Get.SelectFramebox.Hide();
                    }
                }
                else if (currentHandState is HandState.Empty)
                {
                    if (HandOnCell.CanPlantHere && HandDownCell.CanPotAbove && daveHandCanReachMousePos)
                    {
                        ReferenceModel.Get.SelectFramebox.Show();
                        ReferenceModel.Get.SelectFramebox.Position2D(ReferenceModel.Get.Grid.CellToWorld(handCellPos) +
                                                                     ReferenceModel.Get.Grid.cellSize * 0.5f);
                    }
                    else
                    {
                        ReferenceModel.Get.SelectFramebox.Hide();
                    }
                }
                else if (currentHandState is HandState.HaveShovel)
                {
                    if (HandOnCell.HavePlant)
                    {
                        ReferenceModel.Get.SelectFramebox.Show();
                        ReferenceModel.Get.SelectFramebox.Position2D(ReferenceModel.Get.Grid.CellToWorld(handCellPos) +
                                                                     ReferenceModel.Get.Grid.cellSize * 0.5f);
                    }
                    else
                    {
                        ReferenceModel.Get.SelectFramebox.Hide();
                    }
                }
            }
        }
    }
}