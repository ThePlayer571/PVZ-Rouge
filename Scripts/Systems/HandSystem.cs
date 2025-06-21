using System;
using QFramework;
using TPL.PVZR.Events.HandEvents;
using TPL.PVZR.ViewControllers.Others;
using UnityEngine.XR;

namespace TPL.PVZR.Systems
{
    /// <summary>
    /// 相关：手持的是什么；
    /// 无关：选择手持物品 / 放置手持植物的逻辑
    /// </summary>
    public interface IHandSystem : ISystem
    {
        HandState HandState { get; }
        SeedController PickedSeed { get; }
    }

    public enum HandState
    {
        Empty,
        HaveSeed,
        HaveShovel
    }

    public class HandSystem : AbstractSystem, IHandSystem
    {
        protected override void OnInit()
        {
            this.RegisterEvent<SelectSeedEvent>(e =>
            {
                if (HandState != HandState.Empty) throw new Exception($"接收到了SelectSeedEvent，但HandState：{HandState}");
                PickedSeed = e.SelectedSeed;
                HandState = HandState.HaveSeed;
            });

            this.RegisterEvent<DeselectEvent>(e =>
            {
                if (HandState == HandState.Empty) throw new Exception("接收到了DeselectEvent，但HandState：Empty");
                PickedSeed = null;
                HandState = HandState.Empty;
            });
            this.RegisterEvent<SelectShovelEvent>(e =>
            {
                if (HandState != HandState.Empty) throw new Exception($"接收到了SelectShovelEvent，但HandState：{HandState}");
                HandState = HandState.HaveShovel;
            });
        }

        public SeedController PickedSeed { get; set; }
        public HandState HandState { get; private set; }
    }
}