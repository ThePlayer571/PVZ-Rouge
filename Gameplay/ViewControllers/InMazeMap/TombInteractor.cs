using System;
using TPL.PVZR.Gameplay.ViewControllers.AutoInteractBase;
using TPL.PVZR.Gameplay.ViewControllers.InMazeMap.PopInfo;

namespace TPL.PVZR.Gameplay.ViewControllers.InMazeMap
{
    /// <summary>
    /// 检测鼠标进入，提示关卡信息
    /// </summary>
    public sealed partial class TombInteractor : InteractorPointer
    {
        private Tomb _dependTomb;
        private UITombInfo _displayedInfo = null;

        protected override void Awake()
        {
            base.Awake();
            _dependTomb = callTarget as Tomb;
            if (_dependTomb == null) throw new ArgumentException("This interactor needs to be a Tomb");
            _highlighted.Register(highlighted =>
            {
                if (highlighted)
                {
                    // 避免bug：多个面板同时出现
                    if (_displayedInfo != null)
                    {
                        _displayedInfo.HideInstant();
                    }
                    //
                    _displayedInfo = UITombInfo.Create(new TombData { WorldPosition = transform.position});
                    _displayedInfo.Show();
                }
                else
                {
                    _displayedInfo.HideAndDestroy();
                    // 避免bug：上次的_displayedInfo需保留
                    // _displayedInfo = null;
                }
            });
        }
    }
}