using UnityEngine;
using QFramework;

namespace TPL.PVZR
{
    public class LevelSystem : AbstractSystem
    {
        IGameModel _GameModel;
        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();
        }
    }
}