namespace TPL.PVZR.Helpers.New
{
    public static class EntityIdHelper
    {
        private static int _nextId = 1;
        
        public static int AllocateId()
        {
            return _nextId++;
        }
        
        private static int sortingLayer = 0;

        public static int AllocateZombieSortingLayer()
        {
            sortingLayer += 10;
            if (sortingLayer > 30000) sortingLayer = 0;
            return sortingLayer;
        }

        /// <summary>
        /// 数字大的把数字小的举起来
        /// </summary>
        private static int humanLadderPriority = 0;

        public static int AllocateHumanLadderPriority()
        {
            return humanLadderPriority++;
        }
    }
}