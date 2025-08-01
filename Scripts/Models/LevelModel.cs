using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Others;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Models
{
    public interface ILevelModel : IModel
    {
        // Data
        BindableProperty<int> SunPoint { get; }
        List<SeedData> ChosenSeeds { get; }
        BindableProperty<int> CurrentWave { get; }
        BindableProperty<DayPhaseType> CurrentDayPhase { get; }
        BindableProperty<WeatherType> CurrentWeather { get; }

        // Runtime Definition
        ILevelData LevelData { get; }

        // Methods
        SeedData TryGetSeedDataByIndex(int index);
        void Initialize(ILevelData levelData);
        void Reset();
    }


    public class LevelModel : AbstractModel, ILevelModel
    {
        public BindableProperty<int> SunPoint { get; private set; }

        public List<SeedData> ChosenSeeds { get; private set; }
        public BindableProperty<int> CurrentWave { get; private set; }
        public BindableProperty<DayPhaseType> CurrentDayPhase { get; private set; }
        public BindableProperty<WeatherType> CurrentWeather { get; private set; }

        public ILevelData LevelData { get; private set; }

        public SeedData TryGetSeedDataByIndex(int index)
        {
            if (ChosenSeeds.Count < index) return null;
            var target = ChosenSeeds[index - 1];
            if (target.Index != index) throw new Exception($"ChosenSeeds中的index发生错位");
            return target;
        }

        public void Initialize(ILevelData levelData)
        {
            this.LevelData = levelData;

            this.SunPoint.SetValueWithoutEvent(levelData.InitialSunPoint);
            CurrentWave.Value = 0;
            CurrentDayPhase.SetValueWithoutEvent(levelData.InitialDayPhase);
            CurrentWeather.SetValueWithoutEvent(levelData.InitialWeather);
        }

        public void Reset()
        {
            LevelData = null;
            ChosenSeeds.Clear();
            SunPoint.SetValueWithoutEvent(0);
            CurrentWave.SetValueWithoutEvent(0);
            CurrentDayPhase.SetValueWithoutEvent(DayPhaseType.NotSet);
             CurrentWeather.SetValueWithoutEvent(WeatherType.NotSet);
        }


        protected override void OnInit()
        {
            ChosenSeeds = new List<SeedData>();
            CurrentWave = new BindableProperty<int>(0);
            CurrentDayPhase = new BindableProperty<DayPhaseType>(DayPhaseType.NotSet);
            CurrentWeather = new  BindableProperty<WeatherType>(WeatherType.NotSet);
            SunPoint = new BindableProperty<int>(0);
        }
    }
}