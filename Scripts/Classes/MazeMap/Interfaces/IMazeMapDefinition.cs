namespace TPL.PVZR.Classes.MazeMap.Interfaces
{
    public interface IMazeMapDefinition
    {
        // 基础配置
        MazeMapIdentifier identifier { get; }
        int rowCount { get; }
        int colCount { get; }

        //
        int levelCount { get; }
        int[] eliteLevels { get; }
        int finalLevel { get; }

        //
        (int min, int max) spotCountRangeInLevel { get; }
        (int min, int max) spotCountRangeInFirstLevel { get; }
        
        //
        GeneratorId generatorId { get; }
    }
}