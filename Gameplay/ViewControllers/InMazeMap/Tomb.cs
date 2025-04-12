using System;
using QFramework;
using TPL.PVZR.Architecture.Commands;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Gameplay.Class.Levels;
using TPL.PVZR.Gameplay.Class.MazeMap;
using TPL.PVZR.Gameplay.Class.MazeMap.Core;
using TPL.PVZR.Gameplay.ViewControllers.AutoInteractBase;
using TPL.PVZR.Gameplay.ViewControllers.InMazeMap.PopInfo;
using UnityEngine;

namespace TPL.PVZR.Gameplay.ViewControllers.InMazeMap
{
    public struct TombData
    {
        public Vector3 WorldPosition;
    }

    public class Tomb : Spot
    {
        [SerializeField] private Sprite OpenSprite;
        [SerializeField] private Sprite CloseSprite;
        [SerializeField] private Sprite PassSprite;
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private AutoInteractor attachInteractor;

        private ILevel carryLevel = null;
        public override void Interact()
        {
            this.SendCommand<EnterLevelCommand>(new EnterLevelCommand(LevelHelper.GetLevel(LevelIdentifier.DaveHouse),
                this.nodeCarryingThisSpot));
        }

        public override void Init(Node nodeCarryingThisSpot)
        {
            base.Init(nodeCarryingThisSpot);
            foreach (var passSpotId in _GameModel.currentGame.mazeMapSaveData.passSpotIds)
            {
                passSpotId.LogInfo();
            }
            // [Case 1] 已经通过了
            if (_GameModel.currentGame.mazeMapSaveData.passSpotIds.Contains(nodeCarryingThisSpot.id))
            {
                _spriteRenderer.sprite = PassSprite;
                attachInteractor.SetSelectable(false);
            }
            // [Case 2] 可以被选择
            else if (_GameModel.lastEnteredNode.toKey.Contains(nodeCarryingThisSpot))
            {
                _spriteRenderer.sprite = OpenSprite;
            }
            // [Case 3] 不能被选择
            else
            {
                _spriteRenderer.sprite = CloseSprite;
                attachInteractor.SetSelectable(false);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            //
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}