using System.Collections.Generic;
using TPL.PVZR.Tools;

namespace TPL.PVZR.Classes.MazeMap.Interfaces
{
    public interface IMazeMatrixGenerator
    {
        (Matrix<Node> mazeMatrix, Dictionary<Node, List<Node>>) Generate();
        void ValidateParameters();
    }
}