using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.ViewControllers.Others.LevelScene
{
    public class LevelTilemapNode : MonoBehaviour
    {
        public static LevelTilemapNode Instance { get; private set; }

        [SerializeField] [Tooltip("不可直接种植植物的Block")]
        public Tilemap Ground;

        [SerializeField] [Tooltip("水")] public Tilemap ShallowWater;
        public Tilemap DeepWater;

        [SerializeField] [Tooltip("泥土")] public Tilemap Dirt;

        [SerializeField] [Tooltip("关卡小物件，无碰撞箱，阻挡植物种植")]
        public Tilemap SoftObstacle;

        [SerializeField] [Tooltip("梯子")] public Tilemap Ladder;
        [SerializeField] [Tooltip("边界")] public Tilemap Bound;
        [SerializeField] public Tilemap Debug;

        private void Awake()
        {
            Instance = this;
        }
    }
}