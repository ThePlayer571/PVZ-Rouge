using Unity.IO.LowLevel.Unsafe;

namespace TPL.PVZR.Classes.DataClasses
{
    public class GlobalEntityData
    {
        public float Plant_Default_Health = 300f;
        public float Plant_Peashooter_ShootInterval = 1.5f;
        public float Plant_Peashooter_ShootDistance = 40f;
        public float Plant_MungBeanShooter_ShootInterval = 0.5f;
        public float Plant_Sunflower_SpawnSunInterval = 24f;
        public float Plant_Sunflower_InitialSpawnSunInterval = 5f;
        public float Plant_Wallnut_Health = 4000f;
        public float Plant_Tallnut_Health = 8000f;
        public float Plant_Marigold_SpawnCoinInterval = 24f;
        public float Plant_Marigold_InitialSpawnCoinInterval = 1f;
        public float Plant_Repeater_PeaInterval = 0.1f;
        public float Plant_PotatoMine_GrowTime = 15f;
        public float Plant_PotatoMine_ExplosionRadius = 1f;
        public float Plant_CherryBomb_ExplosionRadius = 2f;
        public float Plant_IcebergLettuce_FreezeRadius = 0.6f;
        public float Plant_GatlingPea_PeaInterval = 0.05f;
        public float Plant_SniperPea_ShootInterval = 3f;
        public float Plant_Jalapeno_ExplosionLength = 20f;
        public float Plant_Squash_Radius = 0.5f;
        public float Plant_BonkChoy_HitInterval = 0.5f;
        public float Plant_CabbagePult_ThrowInterval = 3f;
        public float Plant_CabbagePult_ThrowDistance = 16f;
        public float Plant_KernelPult_ButterChance = 0.25f;
        public float Plant_PuffShroom_ShootDistance = 4f;
        public float Plant_SunShroom_GrowTime = 120f;
        public float Plant_Chomper_ChewTime = 30f;

        public float Projectile_Pea_Speed = 10f;
        public float Projectile_SnipePea_Speed = 20f;
        public int Projectile_Spike_MaxAttackCount = 3;
        public float Projectile_FirePea_AOERadius = 0.3f;
        public float Projectile_Cabbage_Speed = 12f;
        public float Projectile_Cabbage_AngularSpeed = 150f;
        public float Projectile_Melon_AOERadius = 0.5f;

        public float Zombie_Default_Health = 181f;
        public float Zombie_Default_ClimbSpeed = 1.5f;
        public float Zombie_DuckyTubeZombie_SwimSpeedMultiplier = 3f;
    }
}