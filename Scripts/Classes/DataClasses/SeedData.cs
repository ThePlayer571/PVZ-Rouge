using TPL.PVZR.Classes.DataClasses.Card;
using TPL.PVZR.Tools;

namespace TPL.PVZR.Classes.DataClasses
{
    public class SeedData
    {
        // Runtime Definition
        public int Index { get; }
        public CardData CardData { get; }

        // Runtime Data
        public Timer ColdTimeTimer { get; }


        public SeedData(int index, CardData cardData)
        {
            this.Index = index;
            this.CardData = cardData;

            this.ColdTimeTimer = new Timer(cardData.CardDefinition.ColdTime);
            this.ColdTimeTimer.SetRemaining(CardData.CardDefinition.InitialColdTime);
        }
    }
}