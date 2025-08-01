using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others.LevelScene
{
    public class LevelTilemapController : MonoBehaviour, IController
    {
        private void Awake()
        {
            var _LevelGridModel = this.GetModel<ILevelGridModel>();
            _LevelGridModel.OnTileChanged.Register((e) =>
            {
                switch ((e.OldState, e.NewState))
                {
                    case (CellTileState.Gravestone, CellTileState.Empty):

                        break;
                    case (CellTileState.Empty, CellTileState.Gravestone):
                        break;
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}