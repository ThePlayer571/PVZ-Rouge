using System.Threading.Tasks;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.ZombieAI.Class;
using TPL.PVZR.Tools;
using UnityEngine;

namespace TPL.PVZR.Classes.ZombieAI.Public
{
    public interface IZombieAIUnit
    {
        // 初始化
        Task InitializeFromAsync(Matrix<Cell> levelMatrix);

        Task RebakeFromAsync(Matrix<Cell> levelMatrix, out bool isRebakeSuccess);

        // 获取路径
        IZombiePath FindPath(Vector2Int start, Vector2Int end, AITendency aiTendency);
        Cluster GetClusterSafely(Vector2Int pos);

        Vertex GetVertexSafely(Vector2Int pos);

        // Debug
        void DebugDisplayMatrix();
        void DebugLogCluster(Vector2Int pos);

        // 属性
        /// <summary>
        /// 需要重新烘焙的标记。设为true后，应该由System检测并调用烘焙函数。设为true时，认为正在或即将进行烘焙
        /// </summary>
        bool RebakeDirty { get; set; }

        /// <summary>
        /// 获取上次烘焙的时间（单位：秒，float，使用Unity的Time.time）
        /// </summary>
        float LastBakeTime { get; }
    }
}