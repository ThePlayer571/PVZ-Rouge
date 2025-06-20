using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.Level;
using TPL.PVZR.ViewControllers.Others;

namespace TPL.PVZR.Models
{
    public interface ILevelModel : IModel
    {
        // Data
        int SunPoint { get; set; }
        List<CardData> ChosenCardData { get; }
        List<SeedController> ChosenSeedControllers { get; }
        ILevelData LevelData { get; }

        // Methods
        void Initialize(ILevelData levelData);
        void Reset();
    }


    public class LevelModel : AbstractModel, ILevelModel
    {
        public int SunPoint { get; set; }
        public List<CardData> ChosenCardData { get; private set; }
        public List<SeedController> ChosenSeedControllers { get; private set; }
        public ILevelData LevelData { get; private set; }

        public void Initialize(ILevelData levelData)
        {
            this.SunPoint = levelData.InitialSunPoint;

            this.LevelData = levelData;
        }

        public void Reset()
        {
            ChosenCardData.Clear();
            ChosenSeedControllers.Clear();
        }


        protected override void OnInit()
        {
            ChosenCardData = new List<CardData>();
            ChosenSeedControllers = new List<SeedController>();
        }
    }
}