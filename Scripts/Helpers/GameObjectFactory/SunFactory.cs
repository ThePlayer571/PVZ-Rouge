using System;
using System.Threading.Tasks;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.ViewControllers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
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
                SunId.SmallSun => _smallSunPrefab,
                _ => throw new ArgumentOutOfRangeException()
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

    public enum SunId
    {
        Sun,
        SmallSun
    }
}