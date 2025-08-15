using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.Others;
using TPL.PVZR.CommandEvents.Game_EventsForTip;
using TPL.PVZR.Services;
using TPL.PVZR.Tools.Save;

namespace TPL.PVZR.Systems
{
    public interface ITipSystem : ISystem
    {
    }

    public class TipSystem : AbstractSystem, ITipSystem
    {
        private PlayerTipData _playerTipData;

        private ISaveService _SaveService;
        private ITipService _TipService;
        private IPhaseService _PhaseService;

        protected override void OnInit()
        {
            // 初始化
            _PhaseService = this.GetService<IPhaseService>();
            _SaveService = this.GetService<ISaveService>();
            _TipService = this.GetService<ITipService>();

            // 数据加载与存储
            _playerTipData =
                _SaveService.SaveManager.Load<PlayerTipData>(SavePathId.PlayerTipData, new PlayerTipData());
            _PhaseService.RegisterCallBack((GamePhase.MazeMap, PhaseStage.LeaveNormal),
                _ => { _SaveService.SaveManager.Save(SavePathId.PlayerTipData, _playerTipData); });

            // 发送tip事件
            _PhaseService.RegisterCallBack((GamePhase.MazeMapInitialization, PhaseStage.EnterEarly),
                _ => { TrySendTip(TipId.FirstEnterGame); });
        }

        private void TrySendTip(TipId tipId)
        {
            if (_playerTipData.SentTips.Add(tipId))
            {
                _TipService.AddTip(tipId);
            }
        }
    }

    [Serializable]
    public class PlayerTipData : ISaveData
    {
        public HashSet<TipId> SentTips = new();
    }
}