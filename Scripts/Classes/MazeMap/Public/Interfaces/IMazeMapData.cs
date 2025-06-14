using TPL.PVZR.Core;

namespace TPL.PVZR.Classes.MazeMap
{
    public interface IMazeMapData
    {
        // Definition
        IMazeMapDefinition definition { get; }
        
        // Runtime Data
        ulong generatedSeed { get; }
        Matrix<Node> mazeMatrix { get; }
        Node startNode { get; }
        //
    }
}