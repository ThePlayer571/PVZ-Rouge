using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.Systems.Level_Data
{
    /// <summary>
    /// 相关：手持的是什么；
    /// 无关：选择手持物品 / 放置手持植物的逻辑
    /// </summary>
    public interface IHandSystem : IDataSystem
    {
        BindableProperty<HandInfo> HandInfo { get; }
    }


    public class HandSystem : AbstractSystem, IHandSystem
    {
        public BindableProperty<HandInfo> HandInfo { get; private set; }

        private void Reset()
        {
            HandInfo.Value = new HandInfo(HandState.Empty, null);
        }

        protected override void OnInit()
        {
            HandInfo = new BindableProperty<HandInfo>(new HandInfo(HandState.Empty, null));

            var phaseService = this.GetService<IPhaseService>();
            phaseService.RegisterCallBack((GamePhase.LevelExiting, PhaseStage.LeaveNormal), e => { Reset(); });

            this.RegisterEvent<OnShovelUsed>(e =>
            {
                //
                HandInfo.Value = new HandInfo(HandState.Empty, null);
            });

            this.RegisterEvent<OnSeedInHandPlanted>(e =>
            {
                //
                HandInfo.Value = new HandInfo(HandState.Empty, null);
            });
        }
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
}