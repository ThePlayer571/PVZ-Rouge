using QAssetBundle;
using QFramework;
using TPL.PVZR.Helpers;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using TPL.PVZR.Systems.Level_Event;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.__NewlyAdded__
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

            // 僵尸工厂的处理
            EntityFactory.ZombieFactory.RemoveZombie(zombie);

            // 尝试结束关卡
            bool pass = _LevelModel.CurrentWave.Value == _LevelModel.LevelData.TotalWaveCount &&
                        _ZombieSpawnSystem.ActiveTasksCount == 0 &&
                        EntityFactory.ZombieFactory.ActiveZombies.Count == 0;
            if (pass)
            {
                "结束关卡".LogInfo();
                var resLoader = ResLoader.Allocate();
                // 生成LevelEndObject
                var go = resLoader.LoadSync<GameObject>(Levelendobject_prefab.BundleName,
                    Levelendobject_prefab.LevelEndObject).Instantiate(zombie.transform.position, Quaternion.identity);
                _PhaseModel.ChangePhase(GamePhase.AllEnemyKilled);
            }
            else
            {
                $"尝试结束关卡失败：lastWave: {_LevelModel.CurrentWave.Value == _LevelModel.LevelData.TotalWaveCount}, zombieSpawnerCount: {_ZombieSpawnSystem.ActiveTasksCount}, zombieCount: {EntityFactory.ZombieFactory.ActiveZombies.Count}"
                    .LogInfo();
            }
        }
    }
}