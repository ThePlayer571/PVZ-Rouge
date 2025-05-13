using System.Collections.Generic;
using TPL.PVZR.Core.Random;

namespace TPL.PVZR.Gameplay.Class.ZombieAI.ZombieAiUnit
{
    public class AITendency
    {
        #region 数据结构


        public readonly float seed;
        public AllowedPassHeight minPassHeight;
        
        #endregion

        #region 公有方法

        /// <summary>
        /// 删除paths中不成立的path（依据是AITendency的数据结构）
        /// </summary>
        /// <param name="paths"></param>
        public void ApplyFilter(List<Path> paths)
        {
            
        }

        public Path ChooseOnePath(List<Path> paths)
        {
            
        }
        

        #endregion
        
        #region 构造函数

        

        public AITendency(AllowedPassHeight minPassHeight = AllowedPassHeight.TwoAndMore,
            bool chooseClosestPath = false)
        {
            this.minPassHeight = minPassHeight;
            seed = (float)RandomHelper.Default.Value;
        }
        #endregion
    }
}