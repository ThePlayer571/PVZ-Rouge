using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.ViewControllers.Others.LevelScene
{
    public class LevelTilemapController : MonoBehaviour, IController
    {
        private AsyncOperationHandle<TileBase> _ladderTileHandle;

        private void Awake()
        {
            var _LevelGridModel = this.GetModel<ILevelGridModel>();
            _ladderTileHandle = Addressables.LoadAssetAsync<TileBase>("LadderTile");

            _LevelGridModel.OnTileChanged.Register(async e =>
            {
                if (e.NewState == CellTileState.Ladder)
                {
                    var tilemap = LevelTilemapNode.Instance.Ladder;
                    await _ladderTileHandle.Task;
                    tilemap.SetTile(new Vector3Int(e.x, e.y, 0), _ladderTileHandle.Result);
                }
                else if (e is { OldState: CellTileState.Ladder, NewState: CellTileState.Empty })
                {
                    var tilemap = LevelTilemapNode.Instance.Ladder;
                    tilemap.SetTile(new Vector3Int(e.x, e.y, 0), null);
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        private void OnDestroy()
        {
            _ladderTileHandle.Release();
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}