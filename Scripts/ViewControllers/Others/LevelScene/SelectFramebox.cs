using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses;
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

        private bool ShouldDisplay()
        {
            if (HandHelper.IsHandOnUI()) return false;
            if (_PhaseModel.GamePhase != GamePhase.Gameplay) return false;
            switch (_HandSystem.HandInfo.Value.HandState)
            {
                case HandState.Empty:
                    return HandHelper.DaveCanReachHand();
                case HandState.HaveShovel:
                {
                    var handCellPos = HandHelper.HandCellPos();
                    if (!HandHelper.DaveCanReachHand()) return false;
                    if (!_LevelGridModel.IsValidPos(handCellPos)) return false;
                    var handOnCell = _LevelGridModel.GetCell(handCellPos);
                    return handOnCell.CellPlantState == CellPlantState.HavePlant;
                }
                case HandState.HaveSeed:
                {
                    var handCellPos = HandHelper.HandCellPos();
                    if (!HandHelper.DaveCanReachHand()) return false;
                    if (!_LevelGridModel.IsValidPos(handCellPos))
                    {
                        return false;
                    }
                    var plantId = _HandSystem.HandInfo.Value.PickedSeed.CardData.CardDefinition.Id;
                    return _LevelGridModel.CanSpawnPlantOn(handCellPos, plantId);
                }
            }

            throw new Exception($"ShouldDisplay出错，出现未考虑的情况");
        }

        private void Update()
        {
            UpdateView(ShouldDisplay());
        }

        private void UpdateView(bool display)
        {
            if (display)
            {
                _SpriteRenderer.enabled = true;
                transform.position = LevelGridHelper.CellToWorld(HandHelper.HandCellPos());
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