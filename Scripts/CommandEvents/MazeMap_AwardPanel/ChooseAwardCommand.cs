using System;
using QFramework;
using TPL.PVZR.Systems.MazeMap;

namespace TPL.PVZR.CommandEvents.MazeMap_AwardPanel
{
    public struct ChooseAwardEvent
    {
        public int index;
    }

    public class ChooseAwardCommand : AbstractCommand
    {
        public ChooseAwardCommand(int index)
        {
            this.index = index;
        }

        private int index;

        protected override void OnExecute()
        {
            var AwardSystem = this.GetSystem<IAwardSystem>();

            if (!AwardSystem.IsAwardAvailable)
                throw new Exception($"调用了 {nameof(ChooseAwardCommand)}，但AwardSystem.IsAwardAvailable: false");

            this.SendEvent<ChooseAwardEvent>(new ChooseAwardEvent { index = this.index });
        }
    }
}