using System;
using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers;

namespace TPL.PVZR.CommandEvents.Level_Gameplay
{
    public class CollectSunCommand : AbstractCommand
    {
        public CollectSunCommand(Sun sun)
        {
            this._sun = sun;
        }

        private Sun _sun;

        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();
            var _LevelModel = this.GetModel<ILevelModel>();

            if (_PhaseModel.GamePhase is not (GamePhase.Gameplay or GamePhase.AllEnemyKilled))
                throw new Exception($"尝试调用CollectSunCommand，但GameState: {_PhaseModel.GamePhase}"); // 游戏阶段正确
            if (_sun == null) throw new ArgumentException("尝试调用CollectSunCommand，但Sun对象为null"); // Sun对象不为null

            _LevelModel.SunPoint.Value += _sun.SunPoint;
            _sun.OnCollected();
        }
    }
}