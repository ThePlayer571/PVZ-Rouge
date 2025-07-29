using System.Linq.Expressions;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;

namespace TPL.PVZR.ViewControllers.Entities.Plants
{
    public interface ISleepyShroom : IPlant
    {
        bool IsAwake { get; }
        void Awaken();
    }

    /// <summary>
    /// 自动处理睡眠
    /// </summary>
    public abstract class SleepyMushroomBase : Plant, ISleepyShroom
    {
        protected sealed override void OnInit()
        {
            _awake = _LevelModel.CurrentDayPhase.Value.ShouldMushroomAwake();
            OnShroomInit();
        }

        protected sealed override void OnUpdate()
        {
            if (_awake) OnShroomUpdate();
        }

        protected virtual void OnShroomInit()
        {
        }

        protected virtual void OnShroomUpdate()
        {
        }


        protected bool _awake;
        public bool IsAwake => _awake;

        public void Awaken()
        {
            if (!_awake)
            {
                _awake = true;
            }
        }
    }
}