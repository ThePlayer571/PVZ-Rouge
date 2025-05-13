namespace TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit
{
    public interface IZombieAIUnit
    {
        // 初始化
        void InitializeFromMap();
        // 获取路径

        Path FindPath(Vertex startVertex, Vertex endVertex, AITendency aiTendency);
    }
}