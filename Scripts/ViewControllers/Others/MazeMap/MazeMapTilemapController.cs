using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.ViewControllers.Others.MazeMap
{
    public class MazeMapTilemapController : MonoBehaviour
    {
        public static MazeMapTilemapController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField] public Tilemap Ground;
    }
}