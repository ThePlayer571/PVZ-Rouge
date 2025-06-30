namespace TPL.PVZR.Classes.DataClasses.Card
{
    public class CardData
    {
        public CardDefinition CardDefinition { get; }

        public CardData(CardDefinition cardDefinition)
        {
            this.CardDefinition = cardDefinition;
        }
    }
}