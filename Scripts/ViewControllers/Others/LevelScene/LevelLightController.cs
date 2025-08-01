using QFramework;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
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
            }

            if (_LevelModel.CurrentWeather.Value == WeatherType.Rainy)
            {
                globalLightIntensity -= 0.2f;
                globalParallaxLightIntensity -= 0.05f;
            }

            GlobalLight.intensity = globalLightIntensity;
            GlobalParallaxLight.intensity = globalParallaxLightIntensity;
            PlayerLight.enabled = playerLightEnabled;
        }

        public void Awake()
        {
            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                SetUpLights();
                                break;
                        }

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