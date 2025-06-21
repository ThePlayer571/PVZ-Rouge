using System;
using TPL.PVZR.Core;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.ViewControllers.Others
{
    public class LevelTilemapController : MonoBehaviour
    {
        [SerializeField] public Tilemap BackGround;
        [SerializeField] public Tilemap Ground;
        [SerializeField] public Tilemap Dirt;
        [SerializeField] public Tilemap Bound;

        private void Awake()
        {
            ReferenceHelper.LevelTilemap = this;
        }
    }
}