using System;
using QFramework;
using TPL.PVZR.Classes.Game;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;

namespace TPL.PVZR.Commands
{
    public class StartRandomNewGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            // 异常处理
            var PhaseModel = this.GetModel<PhaseModel>();
            if (PhaseModel.GamePhase != GamePhase.MainMenu) throw new Exception($"在不正确的阶段执行StartRandomNewGameCommand：{PhaseModel.GamePhase}");
            
            // 生成一个新的游戏数据
            var GameData = new GameData();
            GameData.seed = RandomHelper.Default.NextUnsigned();
            
            // 开始游戏
            var GameSystem = this.GetSystem<IGameSystem>();
            GameSystem.StartGame(GameData);
        }
    }
    
    
    
    public class StartGameCommand:AbstractCommand
    {
        protected override void OnExecute()
        {
            throw new System.NotImplementedException();
        }
    }
}