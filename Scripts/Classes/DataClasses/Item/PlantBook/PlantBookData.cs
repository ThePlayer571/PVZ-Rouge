namespace TPL.PVZR.Classes.DataClasses.Item.PlantBook
{
    public class PlantBookData : ItemData
    {
        public override ItemType ItemType { get; } = ItemType.PlantBook;
        
        
        public PlantId Id { get; }
        public PlantVariant Variant { get; }

        public PlantBookData(PlantBookInfo info)
        {
            Id = info.Id;
            Variant = info.Variant;
        }
    }
}