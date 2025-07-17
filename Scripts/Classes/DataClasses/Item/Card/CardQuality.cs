namespace TPL.PVZR.Classes.DataClasses.Item.Card
{
    public enum CardQuality
    {
        White,
        Green,
        Blue,
        Purple,
        Gold
    }

    public static class Extensions
    {
        public static float ToWeight(this CardQuality quality)
        {
            return quality switch
            {
                CardQuality.White => 20f,
                CardQuality.Green => 10f,
                CardQuality.Blue => 5f,
                CardQuality.Purple => 2f,
                CardQuality.Gold => 1f,
                _ => 0f
            };
        }
    }
}