using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.CommandEvents.Level_Gameplay.Coins;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.ViewControllers;
using TPL.PVZR.ViewControllers.Entities.Projectiles;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
    public static class EntityFactory
    {
        private static readonly ResLoader _resLoader;

        static EntityFactory()
        {
            ResKit.Init();
            _resLoader = ResLoader.Allocate();
        }

        #region SunFactory

        public static class SunFactory
        {
            public static async Task InitializeAsync()
            {
                var sunTask = Addressables.LoadAssetAsync<GameObject>("Sun").Task;
                var smallSunTask = Addressables.LoadAssetAsync<GameObject>("SmallSun").Task;

                await Task.WhenAll(sunTask, smallSunTask);

                _sunPrefab = sunTask.Result;
                _smallSunPrefab = smallSunTask.Result;
            }
    
            private static GameObject _sunPrefab;
            private static GameObject _smallSunPrefab;

            public static Sun SpawnSunWithJump(SunId sunId, Vector2 position, bool autoCollect = true)
            {
                var sunPrefab = sunId switch
                {
                    SunId.Sun => _sunPrefab,
                    SunId.SmallSun => _smallSunPrefab
                };
                var go = sunPrefab.Instantiate(position, Quaternion.identity).GetComponent<Sun>();
                var spriteRenderer = go.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "MidGround";
                spriteRenderer.sortingOrder = 360;


                Vector3 endPos = new Vector3(position.x + (RandomHelper.Default.Range(-0.5f, 0.5f)),
                    position.y + (RandomHelper.Default.Range(0f, 0.2f)), 0);
                go.transform.DOJump(endPos, 1f, 1, 0.5f);

                if (autoCollect)
                {
                    ActionKit.Delay(2f, () => { go.TryCollect(); }).Start(go);
                }

                return go;
            }

            public static Sun SpawnSunWithFall(SunId sunId, Vector2 targetPosition, bool autoCollect = true)
            {
                var sunPrefab = sunId switch
                {
                    SunId.Sun => _sunPrefab,
                    SunId.SmallSun => _smallSunPrefab
                };
                const float topOffset = 1f;
                // 从屏幕顶端之上开始
                var topY = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane)).y;
                var startPosition = new Vector3(targetPosition.x, topY + topOffset, 0);

                var go = sunPrefab.Instantiate(startPosition, Quaternion.identity).GetComponent<Sun>();
                var spriteRenderer = go.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "BackGround";
                spriteRenderer.sortingOrder = 90;

                // 匀速缓慢掉落到目标位置
                var distance = topY - targetPosition.y;
                var duration = distance / 1f;

                go.transform.DOMove(targetPosition, duration).SetEase(Ease.OutQuint);

                if (autoCollect)
                {
                    ActionKit.Delay(duration + 2f, () => { go.TryCollect(); }).Start(go);
                }

                return go;
            }
        }

        #endregion

        #region CoinFactory

        public static class CoinFactory
        {
            public static async Task InitializeAsync()
            {
                var goldTask = Addressables.LoadAssetAsync<GameObject>("CoinGold").Task;
                var silverTask = Addressables.LoadAssetAsync<GameObject>("CoinSilver").Task;

                await Task.WhenAll(goldTask, silverTask);

                _goldCoinPrefab = goldTask.Result;
                _silverCoinPrefab = silverTask.Result;
            }
            
            private static GameObject _silverCoinPrefab;
            private static GameObject _goldCoinPrefab;

            public static Coin SpawnCoinWithJump(CoinId coinId, Vector2 position, bool autoCollect = true)
            {
                var prefab = coinId switch
                {
                    CoinId.Silver => _silverCoinPrefab,
                    CoinId.Gold => _goldCoinPrefab,
                    _ => throw new ArgumentException($"未考虑的硬币类型：{coinId}")
                };
                var go = prefab.Instantiate(position, Quaternion.identity).GetComponent<Coin>();

                Vector3 endPos = new Vector3(position.x + (RandomHelper.Default.Range(-0.5f, 0.5f)),
                    position.y + (RandomHelper.Default.Range(0f, 0.2f)), 0);
                go.transform.DOJump(endPos, 1f, 1, 0.5f);

                if (autoCollect)
                {
                    ActionKit.Delay(2f, () => { go.TryCollect(); }).Start(go);
                }

                return go;
            }
        }

        #endregion

        #region ZombieFactory

       

        #endregion

        #region ProjectileFactory

        public static class ProjectileFactory
        {
            static ProjectileFactory()
            {
                _projectileDict =
                    new Dictionary<ProjectileId, GameObject>
                    {
                        [ProjectileId.Pea] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.Pea),
                        [ProjectileId.FrozenPea] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.FrozenPea),
                        [ProjectileId.MungBean] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.MungBean),
                        [ProjectileId.Spike] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.Spike),
                        [ProjectileId.Star] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.Star),
                        [ProjectileId.SnipePea] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.SnipePea),
                        [ProjectileId.FirePea] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.FirePea),
                        [ProjectileId.Cabbage] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.Cabbage),
                        [ProjectileId.Kernel] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.Kernel),
                        [ProjectileId.Butter] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.Butter),
                        [ProjectileId.Melon] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.Melon),
                        [ProjectileId.ShortSpore] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.ShortSpore),
                        [ProjectileId.Spore] =
                            _resLoader.LoadSync<GameObject>(Projectiles.BundleName, Projectiles.Spore),
                    };
            }

            private static Dictionary<ProjectileId, GameObject> _projectileDict;

            public static Projectile CreatePea(ProjectileId id, Vector2 direction, Vector2 pos)
            {
                if (_projectileDict.TryGetValue(id, out var projectilePrefab))
                {
                    var peaLikeInit = projectilePrefab.GetComponent<IPeaLikeInit>();
                    if (peaLikeInit == null)
                        throw new Exception($"Prefab未实现IPeaLikeInit接口: {projectilePrefab.name}");

                    var go = projectilePrefab.Instantiate(pos, Quaternion.identity).GetComponent<IPeaLikeInit>();
                    go.Initialize(direction.normalized);

                    // 返回Projectile基类
                    var projectile = go as Projectile;
                    return projectile;
                }
                else
                {
                    throw new ArgumentException($"未考虑的投射物类型：{id}");
                }
            }
        }

        #endregion
    }

    public enum SunId
    {
        Sun,
        SmallSun
    }
}