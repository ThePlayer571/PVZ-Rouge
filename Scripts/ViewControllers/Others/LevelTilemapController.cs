using System;
using TPL.PVZR.Tools;
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
        [SerializeField] public Tilemap Debug;

        private void Awake()
        {
            ReferenceHelper.LevelTilemap = this;
        }
    }
}