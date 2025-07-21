using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.CommandEvents.Level_Gameplay.HandInputs;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Models;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.Systems.Level_Data
{
    /// <summary>
    /// 相关：手持的是什么；
    /// 无关：选择手持物品 / 放置手持植物的逻辑
    /// </summary>
    public interface IHandSystem : IServiceManageSystem, IDataSystem
    {
        BindableProperty<HandInfo> HandInfo { get; }
    }


    public class HandSystem : AbstractSystem, IHandSystem
    {
        private void Reset()
        {
            HandInfo.Value = new HandInfo(HandState.Empty, null);
        }
        
        protected override void OnInit()
        {
            HandInfo = new BindableProperty<HandInfo>(new HandInfo(HandState.Empty, null));

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.LevelExiting:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.LeaveNormal:
                                Reset();
                                break;
                        }

                        break;
                }
            });

            #region 关卡内触发事件

            this.RegisterEvent<SelectSeedEvent>(e =>
            {
                HandInfo.Value = new HandInfo(HandState.HaveSeed, e.SelectedSeedData);
            });

            this.RegisterEvent<DeselectEvent>(e => { HandInfo.Value = new HandInfo(HandState.Empty, null); });

            this.RegisterEvent<SelectShovelEvent>(e => { HandInfo.Value = new HandInfo(HandState.HaveShovel, null); });

            this.RegisterEvent<OnSeedInHandPlanted>(e => { HandInfo.Value = new HandInfo(HandState.Empty, null); });

            this.RegisterEvent<OnShovelUsed>(e => { HandInfo.Value = new HandInfo(HandState.Empty, null); });

            #endregion
        }

        public BindableProperty<HandInfo> HandInfo { get; private set; }
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