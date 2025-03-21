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
}