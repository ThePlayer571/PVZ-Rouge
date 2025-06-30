namespace TPL.PVZR.Classes.DataClasses.Level
{
    public interface ILevelData
    {
        int InitialSunPoint { get; }

        GlobalEntityData GlobalEntityData { get; }

        // Definition
        LevelDefinition LevelDefinition { get; }
    }
}