using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
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
    /// 管理墓碑的数据存储和生成
    /// </summary>
    public interface IGravestoneSystem
    {
    }

    public class GravestoneSystem : AbstractSystem, IGravestoneSystem
    {
        private ILevelGridModel _LevelGridModel;
        private Camera _mainCamera;

        private void SpawnGravestone()
        {
        }

        private void Reset()
        {
            _mainCamera = null;
        }

        protected override void OnInit()
        {
            _LevelGridModel = this.GetModel<ILevelGridModel>();

            this.RegisterEvent<OnWaveStart>(e =>
            {
                if (e.Wave > 5 && RandomHelper.Default.NextBool(0.2f))
                {
                    SpawnGravestone();
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
    }
}