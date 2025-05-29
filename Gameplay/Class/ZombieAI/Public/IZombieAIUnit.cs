using TPL.PVZR.Gameplay.Class.ZombieAI.Class;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.Public
{
    public interface IZombieAIUnit
    {
        // 初始化
        void InitializeFromMap();
        // 获取路径
        IPath FindPath(Vertex startVertex, Vertex endVertex, AITendency aiTendency);
    }
}