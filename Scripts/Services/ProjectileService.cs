using System.Threading.Tasks;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.ViewControllers.Entities.Projectiles;
using UnityEngine;

namespace TPL.PVZR.Services
{
    public interface IProjectileService : IService
    {
        Task<Projectile> CreatePea(ProjectileId id, Vector2 direction, Vector2 pos, int? entityId = null);
        void RemoveProjectile(Projectile projectile);

        /// <summary>
        /// 点燃一个投射物（具体细节在projectile类内部实现）
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="ignitionType"></param>
        void Ignite(ICanBeIgnited projectile, IgnitionType ignitionType);

        void ClearCache();
    }

    public class ProjectileService : AbstractService, IProjectileService
    {
        private ProjectileFactory _projectileFactory;

        protected override void OnInit()
        {
            _projectileFactory = new ProjectileFactory();
        }

        public async Task<Projectile> CreatePea(ProjectileId id, Vector2 direction, Vector2 pos, int? entityId = null)
        {
            var projectile = await _projectileFactory.CreatePea(id, direction, pos);
            if (entityId.HasValue)
            {
                projectile.EntityId = entityId.Value;
                $"set id : {entityId.Value}".LogInfo();
            }
            return projectile;
        }

        public void RemoveProjectile(Projectile projectile)
        {
            projectile.gameObject.DestroySelf();
        }

        public void Ignite(ICanBeIgnited projectile, IgnitionType ignitionType)
        {
            projectile.Ignite(ignitionType);
        }

        public void ClearCache()
        {
            _projectileFactory.ClearCache();
        }
    }
}