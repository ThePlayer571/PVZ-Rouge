namespace TPL.PVZR.Classes.Game
{
    public interface IGameData
    {
        ulong Seed { get; }
        int InitialSunPoint { get; set; }
    }
}