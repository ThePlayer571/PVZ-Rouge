using TPL.PVZR.Core;

namespace TPL.PVZR.Classes.MazeMap.Generator
{
    public interface IMazeMatrixGenerator
    {
        Matrix<Node> Generate();
        void ValidateParameters();
    }
}