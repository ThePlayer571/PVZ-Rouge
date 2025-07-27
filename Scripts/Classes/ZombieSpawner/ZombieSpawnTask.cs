using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Classes.ZombieSpawner
{
    [Obsolete]
    public class ZombieSpawnTask
    {
        #region Public

        public bool IsFinished { get; private set; }

        public ZombieSpawnInfo GetRandomZombieSpawnInfo()
        {
            // 规避bug
            if (availableInfos.Count == 0) return null;

            //
            for (var tryCount = 0; tryCount < 100; tryCount++)
            {
                var totalWeight = availableInfos.Sum(item => item.Weight);
                var chosenWeight = RandomHelper.Default.Value * totalWeight;

                // 根据chosenWeight随机选择一个ZombieSpawnInfo
                ZombieSpawnInfo ChosenInfo = null;
                var currentWeight = 0f;
                foreach (var info in availableInfos)
                {
                    currentWeight += info.Weight;
                    if (currentWeight >= chosenWeight)
                    {
                        ChosenInfo = info;
                        break;
                    }
                }

                if (ChosenInfo == null)
                    throw new Exception($"ChosenInfo为null, totalWeight: {totalWeight}, chosenWeight: {chosenWeight}");

                // 判断ChosenInfo的合理性
                var reasonable = currentValue - ChosenInfo.Value > -allowedValueOffset;
                if (reasonable)
                {
                    currentValue -= ChosenInfo.Value;
                    if (currentValue < allowedValueOffset)
                    {
                        IsFinished = true;
                    }

                    return ChosenInfo;
                }
                else
                {
                    availableInfos.Remove(ChosenInfo);
                    if (availableInfos.Count == 0)
                    {
                        IsFinished = true;
                        return null;
                    }
                }
            }

            //todO: 这个异常处理换成returnnull好，现阶段这么写是为了方便进行bug测试
            throw new Exception("GetRandomZombieSpawnInfo超出尝试次数上限");
        }

        #endregion

        #region Config

        private List<ZombieSpawnInfo> availableInfos { get; }
        private float currentValue { get; set; }
        private float allowedValueOffset { get; }

        #endregion

        #region 构造函数

        public ZombieSpawnTask(List<ZombieSpawnInfo> infos, float value)
        {
            availableInfos = infos;
            currentValue = value;
            allowedValueOffset = value * 0.05f;
        }

        #endregion
    }
}