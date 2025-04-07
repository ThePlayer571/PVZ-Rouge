using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Gameplay.Class.MazeMap;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Test
{
    public class Tester:ViewController
    {
        public Tilemap tilemap;
        public List<Tile> tiles;
        private void Awake()
        {
        }
    }
}