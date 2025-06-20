using UnityEngine;

namespace TPL.PVZR.Classes.Level
{
    public class LevelData : ILevelData
    {
        // 需要实时加载的数据，放到这里是为了好用
        public int InitialSunPoint { get; } = 50;
        
        // 配置数据
        public Vector2 InitialPlayerPos { get; } = new Vector2(20, 9);
    }
}