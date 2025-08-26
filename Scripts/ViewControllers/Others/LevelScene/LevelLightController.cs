using QFramework;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.CommandEvents.New.Level_Shit;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TPL.PVZR.ViewControllers.Others.LevelScene
{
    public class LevelLightController : MonoBehaviour, IController
    {
        [SerializeField] private Light2D GlobalLight;
        [SerializeField] private Light2D GlobalParallaxLight;
        private Light2D PlayerLight;


        private void SetUpLights()
        {
            var _LevelModel = this.GetModel<ILevelModel>();
            PlayerLight = Player.Instance.GetComponentInChildren<Light2D>();

            //
            var globalLightIntensity = 1f;
            var globalParallaxLightIntensity = 0.4f;
            var playerLightEnabled = false;

            switch (_LevelModel.CurrentDayPhase.Value)
            {
                case DayPhaseType.Night:
                    globalLightIntensity -= 0.5f;
                    globalParallaxLightIntensity -= 0.2f;
                    playerLightEnabled = true;
                    break;
                case DayPhaseType.MidNight:
                    globalLightIntensity -= 1f;
                    globalParallaxLightIntensity -= 0.2f;
                    playerLightEnabled = true;
                    break;
            }

            if (_LevelModel.CurrentWeather.Value == WeatherType.Rainy)
            {
                globalLightIntensity -= 0.2f;
                globalParallaxLightIntensity -= 0.05f;
            }

            GlobalLight.intensity = Mathf.Clamp(globalLightIntensity, 0, Mathf.Infinity);
            GlobalParallaxLight.intensity = Mathf.Clamp(globalParallaxLightIntensity, 0, Mathf.Infinity);
            PlayerLight.enabled = playerLightEnabled;
        }

        public void Awake()
        {
            this.RegisterEvent<OnLevelGameObjectPrepared>(e => { SetUpLights(); })
                .UnRegisterWhenGameObjectDestroyed(this);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}