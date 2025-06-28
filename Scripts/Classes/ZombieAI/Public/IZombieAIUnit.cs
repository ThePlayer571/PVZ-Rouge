using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Tools;
using UnityEngine;

namespace TPL.PVZR.Classes.ZombieAI.Public
{
    public interface IZombieAIUnit
    {
        // 初始化
        void InitializeFrom(Matrix<Cell> levelMatrix);
        // 获取路径
        IZombiePath FindPath(Vector2Int start, Vector2Int end, AITendency aiTendency);
        // Debug
        void DebugDisplayMatrix();
        void DebugLogCluster(Vector2Int pos);
    }
}