using TPL.PVZR.Tools;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.ViewControllers.Others.LevelScene
{
    public class LevelTilemapController : MonoBehaviour
    {
        public static LevelTilemapController Instance { get; private set; }
        
        [SerializeField] public Tilemap BackGround;
        [SerializeField] public Tilemap Ground;
        [SerializeField] public Tilemap Dirt;
        [SerializeField] public Tilemap Ladder;
        [SerializeField] public Tilemap Bound;
        [SerializeField] public Tilemap Debug;

        private void Awake()
        {
             Instance = this;
        }
    }
}