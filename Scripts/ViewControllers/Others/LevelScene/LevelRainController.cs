using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.CommandEvents.New.Level_Shit;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others.LevelScene
{
    public class LevelRainController : MonoBehaviour, IController
    {
        [SerializeField] private GameObject RainEffect;

        private void Awake()
        {
            this.RegisterEvent<OnLevelGameObjectPrepared>(_ =>
            {
                var _LevelModel = this.GetModel<ILevelModel>();
                var weather = _LevelModel.LevelData.InitialWeather;
                if (weather == WeatherType.Rainy)
                {
                    RainEffect.Show();
                }
            }).UnRegisterWhenGameObjectDestroyed(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}