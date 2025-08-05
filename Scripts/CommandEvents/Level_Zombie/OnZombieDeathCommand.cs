using QAssetBundle;
using QFramework;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Systems.Level_Event;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.CommandEvents._NotClassified_
{
    public class OnZombieDeathCommand : AbstractCommand
    {
        private Zombie zombie;

        public OnZombieDeathCommand(Zombie zombie)
        {
            this.zombie = zombie;
        }

        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();
            var _LevelModel = this.GetModel<ILevelModel>();
            var _ZombieSpawnSystem = this.GetSystem<IZombieSpawnSystem>();

            // 异常处理
            if (_PhaseModel.GamePhase != GamePhase.Gameplay)
                throw new System.Exception($"在不正确的阶段执行OnZombieDeathCommand：{_PhaseModel.GamePhase}");

            // 移除僵尸
            var zombieService = this.GetService<IZombieService>();
            zombieService.RemoveZombie(zombie);

            // 尝试结束关卡
            var gamePhaseChangeService = this.GetService<IGamePhaseChangeService>();
            gamePhaseChangeService.TrySpawnLevelEndObject(zombie.CoreWorldPos);
        }
    }
}