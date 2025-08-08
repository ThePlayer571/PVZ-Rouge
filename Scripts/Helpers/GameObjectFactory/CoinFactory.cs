using System;
using System.Threading.Tasks;
using DG.Tweening;
using QFramework;
using TPL.PVZR.CommandEvents.Level_Gameplay.Coins;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.ViewControllers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TPL.PVZR.Helpers.New.GameObjectFactory
{
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
}