using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.Card;

namespace TPL.PVZR.Helpers
{
    public static class QFEXT
    {

        private const int MaxSize = 35;
        public static bool HasSlot(this BindableList<CardData> cards, int count = 1)
        {
            return MaxSize - cards.Count >= count;
        }
    }
}