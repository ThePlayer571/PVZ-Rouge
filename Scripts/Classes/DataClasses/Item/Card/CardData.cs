using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Tools.Save;

namespace TPL.PVZR.Classes.DataClasses.Item.Card
{
    public class CardData : ItemData, ISavable<CardSaveData>
    {
        public override ItemType ItemType { get; } = ItemType.Card;
        public CardDefinition CardDefinition { get; set; }
        public bool Locked { get; set; } = false;

        public CardData(CardDefinition cardDefinition, bool locked)
        {
            this.CardDefinition = cardDefinition;
            Locked = locked;
        }
        
        public CardData(CardSaveData saveData)
        {
            CardDefinition = PlantConfigReader.GetCardDefinition(saveData.plantDef);
            Locked = saveData.locked;
        }

        public CardSaveData ToSaveData()
        {
            return new CardSaveData
            {
                plantDef = CardDefinition.PlantDef,
                locked = Locked
            };
        }
    }
}