using System;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;

namespace TPL.PVZR
{

    public class StartGameCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetSystem<ILevelSystem>().EnterLevel(new LevelDaveHouse());
        }
    }

    public class LevelToStateChooseLootCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var _LevelSystem = this.GetSystem<ILevelSystem>();
            // TODO： 测试代码，记得删
            _LevelSystem.levelState.ChangeState(LevelSystem.LevelState.End);
            if (_LevelSystem.levelState.CurrentStateId != LevelSystem.LevelState.End)
            {
                throw new Exception("在不正确的事件调用了 LevelToStateChooseLootCommand");
            }
            _LevelSystem.levelState.ChangeState(LevelSystem.LevelState.ChooseLoots);
        }
    }

    public class ZombieDeadCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            throw new System.NotImplementedException();
        }
    }
}