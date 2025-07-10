using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses.Recipe;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.ClassCreator;
using TPL.PVZR.Models;

namespace TPL.PVZR.Systems
{
    public interface IStoreSystem : ISystem
    {
        List<RecipeData> ActiveRecipes { get; }
    }

    public class StoreSystem : AbstractSystem, IStoreSystem
    {
        private IGameModel _GameModel;


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
                                var ownedPlants = _GameModel.GameData.InventoryData.Cards
                                    .Where(cardData => !cardData.Locked)
                                    .Select(cardData => cardData.CardDefinition.PlantDef.Id)
                                    .ToHashSet();

                                var recipePool = RecipeHelper.GetRelatedRecipes(ownedPlants);
                                var _ = recipePool.GetRandomOutputs(5)
                                    .Select(recipeInfo => new RecipeData(recipeInfo));
                                ActiveRecipes.AddRange(_);
                                break;
                        }

                        break;
                    case GamePhase.MazeMap:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.LeaveLate:
                                ActiveRecipes.Clear();
                                break;
                        }

                        break;
                }
            });
        }

        public List<RecipeData> ActiveRecipes { get; } = new List<RecipeData>();
    }
}