using QFramework;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Gameplay.Class.MazeMap;
using TPL.PVZR.Gameplay.Class.MazeMap.Core;
using UnityEngine;

namespace TPL.PVZR.Architecture.Systems.InGame
{
    public interface IMazeMapSystem : ISystem
    {
    }
    
    public class MazeMapSystem:AbstractSystem, IMazeMapSystem
    {
        public MazeMap CurrentMazeMap { get; private set; } = null;
        private IGameModel _GameModel;
        private GameObject CurrentMazeMapGrid;
        public MazeMap mazeMap;
        
        private GameObject MazeMapGridTemplate;
        protected override void OnInit()
        {
            MazeMapGridTemplate = ResLoader.Allocate().LoadSync<GameObject>("MazeMapGrid");
            _GameModel = this.GetModel<IGameModel>();
        }
        
    }
}