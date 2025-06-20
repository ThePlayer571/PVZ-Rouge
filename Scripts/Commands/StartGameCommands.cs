using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.Game;
using TPL.PVZR.Core.Random;
using TPL.PVZR.Helpers;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using UnityEngine;

namespace TPL.PVZR.Commands
{
    public class StartNewRandomGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            // 异常处理
            var PhaseModel = this.GetModel<IPhaseModel>();
            if (PhaseModel.GamePhase != GamePhase.MainMenu)
                throw new Exception($"在不正确的阶段执行StartRandomNewGameCommand：{PhaseModel.GamePhase}");

            // 生成一个新的游戏数据
            IGameData GameData = GameHelper.CreateGameData();
            // 开始游戏
            PhaseModel.ChangePhase(GamePhase.GameInitialization,
                new Dictionary<string, object> { { "GameData", GameData } });
            
        }
    }


    public class StartGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            throw new System.NotImplementedException();
        }
    }
}