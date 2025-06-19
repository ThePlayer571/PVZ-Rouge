using System.Collections.Generic;
using TPL.PVZR.Core;

namespace TPL.PVZR.Classes.MazeMap.Generator
{
    public interface IMazeMatrixGenerator
    {
        (Matrix<Node> mazeMatrix, Dictionary<Node, List<Node>>) Generate();
        void ValidateParameters();
    }
}