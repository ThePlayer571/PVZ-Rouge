using System;
using QFramework;
using TPL.PVZR.Architecture;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Gameplay.Class.MazeMap;
using TPL.PVZR.Gameplay.Class.MazeMap.Core;
using TPL.PVZR.Gameplay.ViewControllers.AutoInteractBase;

namespace TPL.PVZR.Gameplay.ViewControllers.InMazeMap
{
    public abstract class Spot:ViewController,IController,IInteractable
    {
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public virtual void Init(Node nodeCarryingThisSpot)
        {
            this.nodeCarryingThisSpot = nodeCarryingThisSpot;
        }

        /// <summary>
        /// 有个Node携带这个Spot，这个Node是fatherNode
        /// </summary>
        public Node nodeCarryingThisSpot { get; protected set; } = null;
        protected IGameModel _GameModel;

        protected virtual void Awake()
        {
            _GameModel = this.GetModel<IGameModel>();
        }

        public abstract void Interact();
    }
}