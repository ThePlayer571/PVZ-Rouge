using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.Others.LevelScene;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Systems.Level_Event
{
    /// <summary>
    /// 环境导致的关卡事件
    /// </summary>
    public interface IEnvironmentSystem : IAutoUpdateSystem
    {
    }


    public class EnvironmentSystem : AbstractSystem, IEnvironmentSystem
    {
        private void StartRunning()
        {
            _sunTimer.SetRemaining(5f);
            _mainCamera = Camera.main;

            GameManager.ExecuteOnUpdate(Update);
        }

        private void StopRunning()
        {
            _mainCamera = null;

            GameManager.StopOnUpdate(Update);
        }

        private Timer _sunTimer;
        private Camera _mainCamera;

        private void Update()
        {
            if (_LevelModel.CurrentDayPhase.Value is DayPhaseType.Day or DayPhaseType.Sunset)
            {
                _sunTimer.Update(Time.deltaTime);

                if (_sunTimer.Ready)
                {
                    var pos = LevelGridHelper.CellToWorld(_LevelModel.LevelData.GetRandomSunFallCellPos());
                    EntityFactory.SunFactory.SpawnSunWithFall(SunId.Sun, pos);
                    _sunTimer.Reset();
                }
            }
        }


        private ILevelModel _LevelModel;
        private ILevelGridModel _LevelGridModel;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _LevelGridModel = this.GetModel<ILevelGridModel>();

            _sunTimer = new Timer(10f);

            var phaseService = this.GetService<IPhaseService>();
            phaseService.RegisterCallBack((GamePhase.Gameplay, PhaseStage.EnterNormal), e => { StartRunning(); });
            phaseService.RegisterCallBack((GamePhase.Gameplay, PhaseStage.LeaveNormal), e => { StopRunning(); });

            this.RegisterEvent<OnWaveStart>(e =>
            {
                if (_LevelModel.CurrentDayPhase.Value is DayPhaseType.Night or DayPhaseType.MidNight && e.Wave > 5 &&
                    RandomHelper.Default.NextBool(0.2f))
                {
                    ActionKit.Delay(3f, () =>
                    {
                        var posLD = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _mainCamera.nearClipPlane));
                        var posRU = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _mainCamera.nearClipPlane));
                        var cellPosLD = LevelGridHelper.WorldToCell(posLD);
                        var cellPosRU = LevelGridHelper.WorldToCell(posRU);
                        var xRange = new Vector2Int(cellPosLD.x, cellPosRU.x);
                        var yRange = new Vector2Int(cellPosLD.y, cellPosRU.y);

                        var idealPosList = new List<Vector2Int>();
                        for (int x = xRange.x; x <= xRange.y; x++)
                        {
                            for (int y = yRange.x; y <= yRange.y; y++)
                            {
                                var cellPos = new Vector2Int(x, y);
                                if (_LevelGridModel.IsValidPos(cellPos) &&
                                    _LevelGridModel.GetCell(cellPos).Is(CellTypeId.Empty) &&
                                    _LevelGridModel.IsValidPos(cellPos.Down()) &&
                                    _LevelGridModel.GetCell(cellPos.Down()).Is(CellTypeId.Block))
                                {
                                    idealPosList.Add(cellPos);
                                }
                            }
                        }

                        if (idealPosList.Count > 0)
                        {
                            var targetCellPos = RandomHelper.Default.RandomChoose(idealPosList);
                            Addressables.LoadAssetAsync<GameObject>("Gravestone").Completed += handle =>
                            {
                                var go = handle.Result.Instantiate(LevelGridHelper.CellToWorld(targetCellPos),
                                    Quaternion.identity);
                                this.SendEvent<OnGravestoneSpawned>(new OnGravestoneSpawned
                                    { CellPos = targetCellPos, Gravestone = go });
                            };
                        }
                    }).Start(GameManager.Instance);
                }
            });
        }
    }

    public struct OnGravestoneSpawned
    {
        public Vector2Int CellPos;
        public GameObject Gravestone;
    }
}