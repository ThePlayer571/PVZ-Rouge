using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Data;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Entities.Plants.Base
{
    public abstract class CabbagePultBase : Plant
    {
        protected enum CultState
        {
            InCold,
            Ready
        }

        // 植物属性
        protected float _cultColdTime = Global.cabbageCultColdTime;
        protected CultState _cultState = CultState.InCold;
        protected float _timer;
        protected LayerMask _layerMask;

        // 植物行为
        protected override void DefaultAI()
        {
            if (_cultState == CultState.InCold)
            {
                _timer += Time.deltaTime;
                if (_timer >= _cultColdTime)
                {
                    _cultState = CultState.Ready;
                    _timer = 0;
                }
            }

            if (_cultState == CultState.Ready)
            {
                var collider = Physics2D.OverlapArea(new Vector2(transform.position.x, -Mathf.Infinity),
                    new Vector2(transform.position.x + direction.ToVector2().x * Global.peashooterRange,
                        Mathf.Infinity), _layerMask);
                if (collider)
                {
                    Throw();
                }
            }
        }

        protected virtual void Throw()
        {
            // 创建实体
            // _EntitySystem.CreateICabbage(ProjectileIdentifier.Cabbage, transform.position, direction);
            // 重置冷却
            _cultState = CultState.InCold;
        }

        protected override void OnAwakeBase()
        {
            _layerMask = LayerMask.GetMask("Zombie", "Barrier", "ZombieShield");
            base.OnAwakeBase();
        }
    }
}