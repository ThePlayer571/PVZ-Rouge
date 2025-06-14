namespace TPL.PVZR.Classes.MazeMap.Instances.DaveHouse
{
    public class DaveHouseMazeMapDefinition : IMazeMapDefinition
    {
        public MazeMapIdentifier identifier => MazeMapIdentifier.DaveHouse;
        public int colCount => 7;
        public int rowCount => 21; // rowCount = levelCount * 2 + 1;

        public int levelCount => 10;
        public int[] eliteLevels { get; } = new[] { 4, 7 };
        public int finalLevel => 10;

        // spotCountRangeInLevel.min = (colCount - 1) / 2;
        // spotCountRangeInLevel.max = (colCount + 3) / 2;
        public (int min, int max) spotCountRangeInLevel => (3, 5);

        // spotCountRangeInFirstLevel.min = (colCount + 1) / 2;
        // spotCountRangeInFirstLevel.max = (colCount + 1) / 2;
        public (int min, int max) spotCountRangeInFirstLevel => (4, 4);


        public GeneratorId generatorId => GeneratorId.DaveHouse;
    }
}