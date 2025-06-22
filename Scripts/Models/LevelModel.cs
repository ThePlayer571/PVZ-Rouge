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
        int SunPoint { get; set; }
        List<CardData> ChosenCardData { get; }

        /// <summary>
        /// 与Scene内的坐标|每个Tilemap的坐标对应
        /// </summary>
        Matrix<Cell> LevelMatrix { get; }

        // Runtime Definition
        ILevelData LevelData { get; }

        // Methods
        void Initialize(ILevelData levelData);
        void Reset();
    }


    public class LevelModel : AbstractModel, ILevelModel
    {
        public int SunPoint { get; set; }

        public List<CardData> ChosenCardData { get; private set; }
        public Matrix<Cell> LevelMatrix { get; private set; }


        public ILevelData LevelData { get; private set; }

        public void Initialize(ILevelData levelData)
        {
            this.LevelData = levelData;

            this.SunPoint = levelData.InitialSunPoint;

            this.LevelMatrix = LevelMatrixHelper.BakeLevelMatrix(ReferenceHelper.LevelTilemap, levelData);
            LevelMatrixHelper.SetDebugTiles(LevelMatrix, ReferenceHelper.LevelTilemap.Debug);
        }

        public void Reset()
        {
            ChosenCardData.Clear();
        }


        protected override void OnInit()
        {
            ChosenCardData = new List<CardData>();
        }
    }
}