namespace TPL.PVZR.Classes.DataClasses.Item.Card
{
    public class CardData : ItemData
    {
        public override ItemType ItemType { get; } = ItemType.Card;
        public CardDefinition CardDefinition { get; set; }
        public bool Locked { get; set; } = false;

        public CardData(CardDefinition cardDefinition, bool locked)
        {
            this.CardDefinition = cardDefinition;
            Locked = locked;
        }
    }
}