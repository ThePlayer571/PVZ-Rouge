using QAssetBundle;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.ViewControllers.Entities.Projectiles;
using UnityEngine;

namespace TPL.PVZR.Helpers
{
    public static class ProjectileHelper
    {
        static ProjectileHelper()
        {
            ResKit.Init();
            var resLoader = ResLoader.Allocate();

            _peaPrefab = resLoader.LoadSync<GameObject>(Pea_prefab.BundleName, Pea_prefab.Pea);
        }

        private static GameObject _peaPrefab;

        public static Pea CreatePea(Direction2 direction)
        {
            var go = _peaPrefab.Instantiate().GetComponent<Pea>();
            go.Initialize(direction);
            return go;
        }
    }
}