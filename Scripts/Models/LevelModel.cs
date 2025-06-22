using System;
using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.Level;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Core;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.ViewControllers.Others;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Models
{
    public interface ILevelModel : IModel
    {
        // Data
        BindableProperty<int> SunPoint { get; set; }
        List<SeedData> ChosenSeeds { get; }

        /// <summary>
        /// 与Scene内的坐标|每个Tilemap的坐标对应
        /// </summary>
        Matrix<Cell> LevelMatrix { get; }

        // Runtime Definition
        ILevelData LevelData { get; }

        // Methods
        SeedData TryGetSeedDataByIndex(int index);
        void Initialize(ILevelData levelData);
        void Reset();
    }


    public class LevelModel : AbstractModel, ILevelModel
    {
        public BindableProperty<int> SunPoint { get; set; }

        public List<SeedData> ChosenSeeds { get; private set; }
        public Matrix<Cell> LevelMatrix { get; private set; }


        public ILevelData LevelData { get; private set; }

        public SeedData TryGetSeedDataByIndex(int index)
        {
            if (ChosenSeeds.Count <= index) return null;
            var target = ChosenSeeds[index - 1];
            if (target.Index != index) throw new Exception($"ChosenSeeds中的index发生错位");
            return target;
        }

        public void Initialize(ILevelData levelData)
        {
            this.LevelData = levelData;

            this.SunPoint.SetValueWithoutEvent(levelData.InitialSunPoint);

            this.LevelMatrix = LevelMatrixHelper.BakeLevelMatrix(ReferenceHelper.LevelTilemap, levelData);
            LevelMatrixHelper.SetDebugTiles(LevelMatrix, ReferenceHelper.LevelTilemap.Debug);
        }

        public void Reset()
        {
            LevelData = null;
            ChosenSeeds.Clear();
            SunPoint.SetValueWithoutEvent(0);
            LevelMatrix = null;
        }


        protected override void OnInit()
        {
            ChosenSeeds = new List<SeedData>();
            SunPoint = new BindableProperty<int>(0);
        }
    }
}