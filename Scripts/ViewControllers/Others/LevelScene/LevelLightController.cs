using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

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

            switch (_LevelModel.CurrentDayPhase.Value)
            {
                case DayPhaseType.Day:
                    GlobalLight.intensity = 1;
                    GlobalParallaxLight.intensity = 0.4f;
                    break;
                case DayPhaseType.Night:
                    GlobalLight.intensity = 0.5f;
                    GlobalParallaxLight.intensity = 0.2f;
                    PlayerLight.enabled = true;
                    break;
            }
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