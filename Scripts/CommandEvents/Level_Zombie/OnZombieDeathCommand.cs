using QAssetBundle;
using QFramework;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
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

            // 僵尸工厂的处理
            EntityFactory.ZombieFactory.RemoveZombie(zombie);

            // 尝试结束关卡
            bool pass = _LevelModel.CurrentWave.Value == _LevelModel.LevelData.TotalWaveCount &&
                        _ZombieSpawnSystem.ActiveTasksCount == 0 &&
                        EntityFactory.ZombieFactory.ActiveZombies.Count == 0;
            if (pass)
            {
                // 生成LevelEndObject
                var zombiePosition = zombie.transform.position;
                Addressables.LoadAssetAsync<GameObject>("LevelEndObject").Completed += handle =>
                {
                    handle.Result.Instantiate(zombiePosition, Quaternion.identity);
                    handle.Release();
                };
                _PhaseModel.ChangePhase(GamePhase.AllEnemyKilled);
            }
        }
    }
}