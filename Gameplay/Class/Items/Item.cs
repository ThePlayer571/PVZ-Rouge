namespace TPL.PVZR.Gameplay.Class.Items
{
    public abstract partial class Item
    {
        // TODO: id的永久存储
        public int id { get; private set; }
        public ItemType itemType { get; protected set; }
    }

    public partial class Item
    {
        public enum ItemType
        {
            Card
        }

        public static int nextItemId { get;private set; }

        protected Item()
        {
            this.id = nextItemId++;
        }
    }
}