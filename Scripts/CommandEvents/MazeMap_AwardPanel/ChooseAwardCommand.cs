using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Loot;
using TPL.PVZR.Helpers;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
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