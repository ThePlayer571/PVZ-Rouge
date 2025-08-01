using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
using TPL.PVZR.CommandEvents.Level_Shit;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.SoyoFramework;
using UnityEngine;

namespace TPL.PVZR.Systems.Level_Event
{
    /// <summary>
    /// 管理墓碑的数据存储和销毁
    /// </summary>
    public interface IGravestoneSystem : ISystem
    {
        public GameObject GetGravestoneObject(Vector2Int cellPos);
    }

    public class GravestoneSystem : AbstractSystem, IGravestoneSystem
    {
        private ILevelGridModel _LevelGridModel;
        private Dictionary<Vector2Int, GameObject> _gravestoneObjects = new();

        private void Reset()
        {
            _gravestoneObjects.Clear();
        }

        protected override void OnInit()
        {
            _LevelGridModel = this.GetModel<ILevelGridModel>();
            this.RegisterEvent<OnGravestoneSpawned>(e => { _gravestoneObjects.Add(e.CellPos, e.Gravestone); });
            this.RegisterEvent<OnGravestoneBroken>(e =>
            {
                if (_gravestoneObjects.TryGetValue(e.CellPos, out var gravestone))
                {
                    _gravestoneObjects.Remove(e.CellPos);
                    gravestone.DestroySelf();
                }
            });

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelExiting:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                Reset();
                                break;
                        }

                        break;
                }
            });
        }

        public GameObject GetGravestoneObject(Vector2Int cellPos)
        {
            return _gravestoneObjects[cellPos];
        }
    }
}