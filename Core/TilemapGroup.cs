using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Core
{
    public class TilemapGroup: MonoBehaviour
    {
        [SerializeField] public Tilemap BackGround;
        [SerializeField] public Tilemap Ground;
        [SerializeField] public Tilemap DirtNotice;
        [SerializeField] public Tilemap Bound;
        [SerializeField] public Tilemap Test;
    }
}