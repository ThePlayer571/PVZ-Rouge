using System;
using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.ViewControllers;

namespace TPL.PVZR.CommandEvents.Level_Gameplay.Coins
{
    [Serializable]
    public enum CoinId
    {
        Silver = 0,
        Gold = 1,
        Diamond = 2,
    }

    public class CollectCoinCommand : AbstractCommand
    {
        public CollectCoinCommand(Coin coin)
        {
            _collectedCoin = coin;
        }

        private Coin _collectedCoin;

        protected override void OnExecute()
        {
            var _PhaseModel = this.GetModel<IPhaseModel>();
            var _GameModel = this.GetModel<IGameModel>();

            if (_PhaseModel.GamePhase is not (GamePhase.Gameplay or GamePhase.AllEnemyKilled))
                throw new Exception($"尝试调用CollectCoinCommand，但GameState: {_PhaseModel.GamePhase}"); // 游戏阶段正确
            if (_collectedCoin == null)
                throw new ArgumentException("尝试调用CollectCoinCommand，但Sun对象为null"); // Sun对象不为null

            _GameModel.GameData.InventoryData.Coins.Value += _collectedCoin.value;
            _collectedCoin.OnCollected();
        }
    }
}