namespace TPL.PVZR.ViewControllers.Entities.Projectiles
{
    public interface ICanBeIgnited : IProjectile
    {
        void Ignite(IgnitionType ignitionType);
    }

    public enum IgnitionType
    {
        Fire,
        GhostFire
    }
}