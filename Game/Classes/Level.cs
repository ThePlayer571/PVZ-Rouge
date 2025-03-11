using UnityEngine;

namespace TPL.PVZR
{
    public abstract class Level
    {
        public abstract Vector2Int size { get; }
        public abstract Vector2 daveInitialPos { get; }
    }

    public class LevelDaveHouse : Level
    {
        public override Vector2Int size { get; } = new Vector2Int(20,20);
        public override Vector2 daveInitialPos { get; } = new Vector2(10,10);
    }
}