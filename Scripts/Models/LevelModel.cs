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
        bool TryGetSeedControllerWithIndex(int index, out SeedController seedController);
        Dictionary<int, CardData> ChosenCardData { get; }
        Dictionary<int, SeedController> ChosenSeedControllers { get; }
        ILevelData LevelData { get; }

        // Methods
        void Initialize(ILevelData levelData);
        void Reset();
    }


    public class LevelModel : AbstractModel, ILevelModel
    {
        public int SunPoint { get; set; }
        public bool TryGetSeedControllerWithIndex(int index, out SeedController seedController)
        {
            return ChosenSeedControllers.TryGetValue(index, out seedController);
        }

        public Dictionary<int, CardData> ChosenCardData { get; private set; }
        public Dictionary<int, SeedController> ChosenSeedControllers { get; private set; }
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
            ChosenCardData = new Dictionary<int, CardData>();
            ChosenSeedControllers = new Dictionary<int, SeedController>();
        }
    }
}