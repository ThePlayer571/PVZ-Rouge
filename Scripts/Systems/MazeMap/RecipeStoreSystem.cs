using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.CommandEvents.__NewlyAdded__;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Helpers.ClassCreator.Item;
using TPL.PVZR.Models;

namespace TPL.PVZR.Systems
{
    public interface IRecipeStoreSystem : ISystem
    {
        RecipeData GetRecipeByIndex(int index);
    }

    public class RecipeStoreSystem : AbstractSystem, IRecipeStoreSystem
    {
        private IGameModel _GameModel;

        private void AutoWriteRecipes()
        {
            var ownedPlants = _GameModel.GameData.InventoryData.Cards
                .Where(cardData => !cardData.Locked)
                .Select(cardData => cardData.CardDefinition.PlantDef.Id)
                .ToHashSet();

            var recipePool = RecipeHelper.GetRelatedRecipes(ownedPlants);
            var _ = recipePool.GetRandomOutputs(8)
                .Select(recipeInfo => new RecipeData(recipeInfo));
            _activeRecipes.AddRange(_);
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
                    var cardData = CardHelper.CreateCardData(PlantBookHelper.GetPlantDef(recipe.output.PlantId));
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