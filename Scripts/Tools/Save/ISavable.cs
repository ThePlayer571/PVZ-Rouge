namespace TPL.PVZR.Tools.Save
{
    public interface ISaveData
    {
        
    }

    public interface ISavable<out TSaveData> where TSaveData : ISaveData
    {
        TSaveData ToSaveData();
    }
}