using TPL.PVZR.Gameplay.Class.ZombieAI.Class;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.Public
{
    public interface IZombieAIUnit
    {
        // 初始化
        void InitializeFromMap();
        // 获取路径
        IZombiePath FindPath(Vector2Int start, Vector2Int end, AITendency aiTendency);
    }
}