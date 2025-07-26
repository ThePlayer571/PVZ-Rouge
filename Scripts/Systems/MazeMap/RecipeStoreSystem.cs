using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Tools;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.Systems.MazeMap
{
    public interface IRecipeStoreSystem : IServiceManageSystem, IDataSystem
    {
        RecipeData GetRecipeByIndex(int index);
    }

    public class RecipeStoreSystem : AbstractSystem, IRecipeStoreSystem
    {
        private IGameModel _GameModel;

        private void AutoWriteRecipes()
        {
            RandomPool<RecipeGenerateInfo, RecipeInfo> recipePool;

            var ownedPlants = _GameModel.GameData.InventoryData.Cards
                .Where(cardData => !cardData.Locked)
                .Select(cardData => cardData.CardDefinition.PlantDef.Id)
                .ToHashSet();

            if (ownedPlants.Count > 0)
            {
                recipePool = TradeCreator.CreateRelatedRecipePool(ownedPlants);
                
                if (recipePool.IsFinished)
                {
                    var lockedPlants = _GameModel.GameData.InventoryData.Cards
                        .Where(cardData => cardData.Locked)
                        .Select(cardData => cardData.CardDefinition.PlantDef.Id)
                        .ToHashSet();
                    recipePool = TradeCreator.CreateRelatedRecipePool(lockedPlants);
                }
            }
            else
            {
                var lockedPlants = _GameModel.GameData.InventoryData.Cards
                    .Where(cardData => cardData.Locked)
                    .Select(cardData => cardData.CardDefinition.PlantDef.Id)
                    .ToHashSet();
                recipePool = TradeCreator.CreateRelatedRecipePool(lockedPlants);
            }


            var chosenRecipes = recipePool.GetRandomOutputs(8)
                .Select(recipeInfo => new RecipeData(recipeInfo));
            _activeRecipes.AddRange(chosenRecipes);
        }

        private void ClearRecipes()
        {
            _activeRecipes.Clear();
        }


        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.MazeMapInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterNormal:
                                AutoWriteRecipes();
                                break;
                        }

                        break;
                    case GamePhase.MazeMap:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.LeaveLate:
                                ClearRecipes();
                                break;
                        }

                        break;
                }
            });

            this.RegisterEvent<BarterEvent>(e =>
            {
                var recipe = GetRecipeByIndex(e.index);
                recipe.used = true;
                // 消耗
                _GameModel.GameData.InventoryData.Coins.Value -= recipe.consumeCoins;
                foreach (var cardId in recipe.consumeCards)
                {
                    var cardData = _GameModel.GameData.InventoryData.Cards
                        .First(card => card.CardDefinition.PlantDef.Id == cardId && !card.Locked);
                    if (cardData != null)
                    {
                        _GameModel.GameData.InventoryData.RemoveCard(cardData);
                    }
                }

                // 添加
                {
                    var cardData = ItemCreator.CreateCardData(recipe.output.PlantId.ToDef());
                    _GameModel.GameData.InventoryData.AddCard(cardData);
                }
            });
        }

        private List<RecipeData> _activeRecipes { get; } = new List<RecipeData>();

        public RecipeData GetRecipeByIndex(int index)
        {
            return _activeRecipes[index];
        }
    }
}