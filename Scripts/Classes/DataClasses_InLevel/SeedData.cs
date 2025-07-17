using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Tools;

namespace TPL.PVZR.Classes.DataClasses_InLevel
{
    public class SeedData
    {
        // Runtime Definition
        public int Index { get; }
        public CardData CardData { get; }

        // Runtime Data
        public Timer ColdTimeTimer { get; }


        private SeedData(int index, CardData cardData)
        {
            this.Index = index;
            this.CardData = cardData;

            this.ColdTimeTimer = new Timer(cardData.CardDefinition.ColdTime);
            this.ColdTimeTimer.SetRemaining(CardData.CardDefinition.InitialColdTime);
        }

        public static SeedData Create(int index, CardData cardData)
        {
            return new SeedData(index, cardData);
        }
    }
}