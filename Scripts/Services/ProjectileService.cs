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
        Task<Projectile> CreatePea(ProjectileId id, Vector2 direction, Vector2 pos);
        void RemoveProjectile(Projectile projectile);void ClearCache();
    }

    public class ProjectileService : AbstractService, IProjectileService
    {
        private ProjectileFactory _projectileFactory;

        protected override void OnInit()
        {
            _projectileFactory = new ProjectileFactory();
        }

        public async Task<Projectile> CreatePea(ProjectileId id, Vector2 direction, Vector2 pos)
        {
            var projectile = await _projectileFactory.CreatePea(id, direction, pos);
            return projectile;
        }

        public void RemoveProjectile(Projectile projectile)
        {
            projectile.gameObject.DestroySelf();
        }

        public void ClearCache()
        {
            _projectileFactory.ClearCache();
        }
    }
}
