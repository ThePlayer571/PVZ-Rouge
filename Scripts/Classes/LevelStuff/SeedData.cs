using TPL.PVZR.Classes.GameStuff;

namespace TPL.PVZR.Classes.LevelStuff
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