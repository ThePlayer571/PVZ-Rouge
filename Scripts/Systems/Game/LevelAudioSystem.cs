using System.Linq;
using QFramework;
using TPL.PVZR.CommandEvents.Level_Gameplay.Waves;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.SoyoFramework;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.Systems
{
    public interface ILevelBgmSystem : IAutoUpdateSystem
    {
    }

    public class LevelBgmSystem : AbstractSystem, ILevelBgmSystem
    {
        private IAudioService _AudioService;
        private IPhaseService _PhaseService;
        private IZombieService _ZombieService;
        private ILevelModel _LevelModel;
        private Tension _tension;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _AudioService = this.GetService<IAudioService>();
            _PhaseService = this.GetService<IPhaseService>();
            _ZombieService = this.GetService<IZombieService>();

            _tension = new Tension();

            // Main
            _PhaseService.RegisterCallBack((GamePhase.Gameplay, PhaseStage.EnterLate), e => { StartRunning(); });
            _PhaseService.RegisterCallBack((GamePhase.LevelExiting, PhaseStage.EnterEarly), e => { StopRunning(); });

            // 暂停
            this.RegisterEvent<OnGamePaused>(_ =>
            {
                // todo 未思考清楚解决方案：加if in level 或 合并关卡与外界的pause按钮
                _AudioService.PauseLevelBGM();
            });
            this.RegisterEvent<OnGameResumed>(e =>
            {
                if (!e.TEMP_stopAudio)
                {
                    _AudioService.ResumeLevelBGM();
                }
            });

            // Tension设置
            _ZombieService.OnZombieCountChanged.Register(count =>
            {
                var tension = Mathf.Clamp01(count / 11f);
                _tension.SetTarget(tension);
                if (_tension.Intensity < tension)
                {
                    _tension.SetTension(tension);
                }
            });
            this.RegisterEvent<OnWaveStart>(e =>
            {
                if (_LevelModel.LevelData.HugeWaves.Contains(e.Wave))
                {
                    _tension.IncreaseTension(1);
                }
            });

            // LevelStateSFX
            this.RegisterEvent<OnWaveStart>(e =>
            {
                if (e.Wave == 1)
                {
                    _AudioService.PlaySFX("event:/Sounds/LevelState/FirstZombieComes");
                }

                if (_LevelModel.LevelData.HugeWaves.Contains(e.Wave))
                {
                    ActionKit.Delay(1f, () => _AudioService.PlaySFX("event:/Sounds/LevelState/FlagRise"))
                        .StartGlobal();
                }
            });
        }

        private void StartRunning()
        {
            _AudioService.PlayLevelBGM(LevelMusicId.Lawn);
            GameManager.ExecuteOnUpdate(Update);
        }

        private void StopRunning()
        {
            _tension.Reset();
            _AudioService.StopLevelBGM();
            GameManager.StopOnUpdate(Update);
        }

        private void Update()
        {
            _tension.Update(Time.deltaTime);
            _AudioService.SetIntensity(_tension.Intensity);
        }

        #region 类

        private class Tension
        {
            // 可以设置标准值：Intensity不断往这个值移动，一秒移动0.01f
            // 提供增加/减少紧张值的方法

            private float _currentIntensity = 0f;
            private float _targetIntensity = 0f;
            private const float MOVE_SPEED = 0.1f; // 每秒移动的速度

            /// <summary>
            /// 设置目标紧张值
            /// </summary>
            /// <param name="target">目标值，会被限制在 [0,1] 范围内</param>
            public void SetTarget(float target)
            {
                _targetIntensity = Mathf.Clamp01(target);
            }

            public void SetTension(float tension)
            {
                _currentIntensity = Mathf.Clamp01(tension);
            }

            /// <summary>
            /// 增加紧张值（直接作用于当前值，然后缓慢恢复到目标值）
            /// </summary>
            /// <param name="amount">增加的数量</param>
            public void IncreaseTension(float amount)
            {
                _currentIntensity = Mathf.Clamp01(_currentIntensity + amount);
            }

            /// <summary>
            /// 减少紧张值（直接作用于当前值，然后缓慢恢复到目标值）
            /// </summary>
            /// <param name="amount">减少的数量</param>
            public void DecreaseTension(float amount)
            {
                _currentIntensity = Mathf.Clamp01(_currentIntensity - amount);
            }

            /// <summary>
            /// 重置紧张值到指定值（直接设置当前值和目标值）
            /// </summary>
            /// <param name="value">重置的值，会被限制在 [0,1] 范围内</param>
            public void Reset(float value = 0f)
            {
                _currentIntensity = Mathf.Clamp01(value);
                _targetIntensity = _currentIntensity;
            }

            // 更新
            public void Update(float deltaTime)
            {
                if (_currentIntensity != _targetIntensity)
                {
                    // 计算移动距离
                    float moveDistance = MOVE_SPEED * deltaTime;

                    // 向目标值移动
                    if (_currentIntensity < _targetIntensity)
                    {
                        _currentIntensity = Mathf.Min(_currentIntensity + moveDistance, _targetIntensity);
                    }
                    else
                    {
                        _currentIntensity = Mathf.Max(_currentIntensity - moveDistance, _targetIntensity);
                    }
                }
            }

            // 这是紧张的程度 值域是[0,1]
            public float Intensity => _currentIntensity;
        }

        #endregion
    }
}