using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.ViewControllers.Others.MazeMap
{
    public class MazeMapTilemapNode : MonoBehaviour
    {
        public static MazeMapTilemapNode Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        [SerializeField] public Tilemap Ground;
        [SerializeField] public Transform cameraTarget;
    }
}