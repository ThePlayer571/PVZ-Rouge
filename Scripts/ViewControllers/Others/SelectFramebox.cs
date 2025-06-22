using System;
using QFramework;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Events;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others
{
    public class SelectFramebox : MonoBehaviour, IController
    {
        private SpriteRenderer _SpriteRenderer;
        private IHandSystem _HandSystem;
        private ILevelGridModel _LevelGridModel;
        private IPhaseModel _PhaseModel;

        private void Awake()
        {
            _SpriteRenderer = this.GetComponent<SpriteRenderer>();

            _HandSystem = this.GetSystem<IHandSystem>();
            _LevelGridModel = this.GetModel<ILevelGridModel>();
            _PhaseModel = this.GetModel<IPhaseModel>();
        }

        private void Update()
        {
            if (_PhaseModel.GamePhase != GamePhase.Gameplay)
            {
                UpdateView(false);
            }
            else if (_HandSystem.HandInfo.Value.HandState == HandState.Empty)
            {
                if (HandHelper.PlayerCanReachMouse())
                {
                    UpdateView(true);
                }
                else
                {
                    UpdateView(false);
                }
            }
            else if (_HandSystem.HandInfo.Value.HandState == HandState.HaveShovel)
            {
                var handOnCell = _LevelGridModel.HandOnCell();

                if (!HandHelper.PlayerCanReachMouse())
                {
                    UpdateView(false);
                }
                else if (handOnCell == null)
                {
                    UpdateView(false);
                }
                else if (handOnCell.CellPlantState == CellPlantState.HavePlant)
                {
                    UpdateView(true);
                }
                else
                {
                    UpdateView(false);
                }
            }
            else if (_HandSystem.HandInfo.Value.HandState == HandState.HaveSeed)
            {
                if (!HandHelper.PlayerCanReachMouse())
                {
                    UpdateView(false);
                }
                else if (_LevelGridModel.CanSpawnPlantOn(HandHelper.MouseCellPos(),
                             _HandSystem.HandInfo.Value.PickedSeed.CardData.CardDefinition.Id))
                {
                    UpdateView(true);
                }
                else
                {
                    UpdateView(false);
                }
            }
        }

        private void UpdateView(bool display)
        {
            if (display)
            {
                _SpriteRenderer.enabled = true;
                transform.position = LevelTilemapHelper.CellToWorld(HandHelper.MouseCellPos());
            }
            else
            {
                _SpriteRenderer.enabled = false;
            }
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}