using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.CommandEvents.New.Level_Shit;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others.LevelScene
{
    public class LevelSkyController : MonoBehaviour, IController
    {
        [SerializeField] private GameObject Day;
        [SerializeField] private GameObject Night;

        private void Awake()
        {
            this.RegisterEvent<OnLevelGameObjectPrepared>(_ =>
            {
                var _LevelModel = this.GetModel<ILevelModel>();
                var dayPhase = _LevelModel.LevelData.InitialDayPhase;
                //
                Day.Hide();
                Night.Hide();
                switch (dayPhase)
                {
                    case DayPhaseType.Day:
                        Day.Show();
                        break;
                    case DayPhaseType.Night:
                        Night.Show();
                        break;
                    default:
                        $"出现未考虑的dayPhase: {dayPhase}".LogError();
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