using TPL.PVZR.Classes.DataClasses.Item;

namespace TPL.PVZR.Classes.DataClasses.Card
{
    public class CardData : ItemData
    {
        public override ItemType ItemType { get; } = ItemType.Card;
        public CardDefinition CardDefinition { get; }

        public CardData(CardDefinition cardDefinition)
        {
            this.CardDefinition = cardDefinition;
        }
    }
}