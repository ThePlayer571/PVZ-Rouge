using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs;
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
        BindableProperty<HandInfo> HandInfo { get; }
    }

    public struct HandInfo
    {
        public HandState HandState;
        public SeedData PickedSeed;

        public HandInfo(HandState handState, SeedData pickedSeed)
        {
            this.HandState = handState;
            this.PickedSeed = pickedSeed;
        }
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
            HandInfo = new BindableProperty<HandInfo>(new HandInfo(HandState.Empty, null));

            this.RegisterEvent<SelectSeedEvent>(e =>
            {
                HandInfo.Value = new HandInfo(HandState.HaveSeed, e.SelectedSeedData);
            });

            this.RegisterEvent<DeselectEvent>(e => { HandInfo.Value = new HandInfo(HandState.Empty, null); });

            this.RegisterEvent<SelectShovelEvent>(e => { HandInfo.Value = new HandInfo(HandState.HaveShovel, null); });

            this.RegisterEvent<PlantingSeedInHandEvent>(e => { HandInfo.Value = new HandInfo(HandState.Empty, null); });

            this.RegisterEvent<UseShovelEvent>(e => { HandInfo.Value = new HandInfo(HandState.Empty, null); });
        }

        public BindableProperty<HandInfo> HandInfo { get; private set; }
    }
}