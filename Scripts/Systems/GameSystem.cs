using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.CommandEvents.Phase;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Models;
using TPL.PVZR.Tools.Random;
using TPL.PVZR.Tools.SoyoFramework;

namespace TPL.PVZR.Systems
{
    public interface IGameSystem : IMainSystem
    {
    }

    public class GameSystem : AbstractSystem, IGameSystem
    {
        private IPhaseModel _PhaseModel;
        private IGameModel _GameModel;

        protected override void OnInit()
        {
            _PhaseModel = this.GetModel<IPhaseModel>();
            _GameModel = this.GetModel<IGameModel>();

            this.RegisterEvent<OnPhaseChangeEvent>(e =>
            {
                switch (e.GamePhase)
                {
                    case GamePhase.GameInitialization:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.EnterEarly:
                            {
                                // 数据设置
                                _GameModel.GameData = e.Parameters["GameData"] as IGameData;
                                // 史山：为了其他地方的代码优雅而设
                                {
                                    PlantDefHelper.SetInventory(_GameModel.GameData.InventoryData);
                                    RandomHelper.SetGame(_GameModel.GameData);
                                }
                                break;
                            }
                            case PhaseStage.EnterLate:
                                _PhaseModel.DelayChangePhase(GamePhase.MazeMapInitialization);
                                break;
                        }

                        break;
                    case GamePhase.GameExiting:
                        switch (e.PhaseStage)
                        {
                            case PhaseStage.LeaveNormal:
                            {
                                _GameModel.Reset();
                                // 史山：为了其他地方的代码优雅而设
                                {
                                    PlantDefHelper.SetInventory(null);
                                    RandomHelper.SetGame(null);
                                }
                                break;
                            }
                        }

                        break;
                }
            });
        }
    }
}