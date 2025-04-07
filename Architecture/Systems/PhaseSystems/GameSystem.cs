using System;
using QFramework;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.Interfaces;
using TPL.PVZR.Gameplay.Class.Games;
using TPL.PVZR.Gameplay.Class.MazeMap;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TPL.PVZR.Architecture.Systems.PhaseSystems
{
    public interface IGameSystem : ISystem, IPhaseManageSystem
    {
        /// <summary>
        /// 设置当前的游戏（指缓存中的游戏，进入到GamePreInit会加载该游戏）
        /// </summary>
        /// <param name="game"></param>
        /// <remarks>只能在R:MainGame阶段调用</remarks>
        void SetCurrentGame(IGame game);
    }

    public class GameSystem : AbstractSystem, IGameSystem
    {
        # region IGameSystem
        
        public void SetCurrentGame(IGame game)
        {
            if (_GamePhaseSystem.currentRoughGamePhase is not GamePhaseSystem.RoughGamePhase.MainMenu)
            {
                throw new Exception($"在不正确的时间调用：{_GamePhaseSystem.currentRoughGamePhase}");
            }
            _currentGame = game;
        }
        private IGame _currentGame = null;
        # endregion

        # region IPhaseManageSystem
        
        private void RegisterPhaseEvents()
        {
            RegisterGameInitialization();
            RegisterMazeMap();
        }

        private void RegisterGameInitialization()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.GameInitialization)
                {
                    ActionKit.Sequence()
                        .Callback(()=>SceneTransitionManager.Instance.AddMaskReason("GameInitialization"))
                        .Condition(() => SceneTransitionManager.Instance.isMask)
                        .Callback(() =>
                        {
                            if (_currentGame is null) throw new Exception("尝试开始游戏，但是没有设置currentGame");
                            _GamePhaseSystem.ChangePhase(GamePhaseSystem.GamePhase.MazeMap);
                        })
                        .Start(GameManager.Instance);
                }
            });
        }

        private void RegisterMazeMap()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.MazeMap)
                {
                    AsyncOperation ao = null;
                    ActionKit.Sequence()
                        .Callback(()=>SceneTransitionManager.Instance.AddMaskReason("MazeMap"))
                        .Condition(() => SceneTransitionManager.Instance.isMask)
                        .Callback(() => { ao = SceneManager.LoadSceneAsync("GameMapSceneTest"); })
                        .Condition(() => ao.isDone)
                        .Callback(() => // 生成构建地图
                        {
                            var mazeMap = MazeMapHelper.CreateMazeMap(_GameModel.currentGame.mazeMapCreateData);
                        })
                        .Callback(() => { SceneTransitionManager.Instance.RemoveMaskReason("MazeMap"); })
                        .Start(GameManager.Instance);
                }
            });
        }

        #endregion
        
        # region 私有
        // 引用
        private IGameModel _GameModel;
        private IGamePhaseSystem _GamePhaseSystem;
        
        // 初始化
        protected override void OnInit()
        {
            _GameModel = this.GetModel<IGameModel>();
            _GamePhaseSystem = this.GetSystem<IGamePhaseSystem>();
            RegisterPhaseEvents();
        }
        
        # endregion
        
    }
}