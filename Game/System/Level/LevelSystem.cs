using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;

namespace TPL.PVZR
{
    public interface ILevelSystem
    {
        public FSM<LevelSystem.LevelState> levelState { get; }
        public bool canAddCard { get; }
    }
    
    
    // 构建关卡/管理关卡进程
    public class LevelSystem : AbstractSystem,ILevelSystem
    {
        # region 通用
        public enum LevelState
        {
            OutOfLevel,BuildingLevel, ChoosingCards, Gameplay, End 
        }

        public FSM<LevelState> levelState { get; private set; }
        private ILevelModel _LevelModel;
        private InputSystem _InputSystem;
        private IHandSystem _HandSystem;
        
        // 为方便调用而存的变量
        private UILevelChooseCardPanel _UILevelChooseCardPanel;
        private UIGamePanel _UIGamePanel;
        
        protected override void OnInit()
        {
            // Temp
            "this init".LogInfo();
            cards = new List<Card>();
            //
            _LevelModel = this.GetModel<ILevelModel>();
            _InputSystem = this.GetSystem<InputSystem>();
            _HandSystem = this.GetSystem<IHandSystem>();
            //
            levelState = new FSM<LevelState>();
            //
            SetUpState();
        }

        private Level _levelToBuild;

        private void SetUpState()
        {
            // 
            levelState.State(LevelState.BuildingLevel)
                .OnEnter(() =>
                {
                    AsyncOperation ao = new();
                    //
                    var _ResLoader = ResLoader.Allocate();
                    // 切换至空场景
                    ActionKit.Sequence()
                        .Callback(SceneTransitionManager.Hide)
                        .Delay(1f)
                        .Callback(() => // 搭建场景＆基础设置
                        {
                            Time.timeScale = 0;
                            ao = SceneManager.LoadSceneAsync("LevelSceneTemplate");
                            _LevelModel.SetLevel(_levelToBuild);
                        })
                        .Condition(() => ao.isDone) // 成功构建场景
                        .Callback(() => // 配置关卡GameObject
                        {
                            // Dave
                            var _Dave = _ResLoader.LoadSync<Dave>("Dave")
                                .Instantiate(_levelToBuild.daveInitialPos, Quaternion.identity);
                            // VirtualCamara Confiner
                            var _ConfinerPolygonCollider =
                                GameObject.Find("Confiner").GetComponent<PolygonCollider2D>();
                            _ConfinerPolygonCollider.points[1] = new Vector2(_levelToBuild.size.x, 0);
                            _ConfinerPolygonCollider.points[2] =
                                new Vector2(_levelToBuild.size.x, _levelToBuild.size.y);
                            _ConfinerPolygonCollider.points[3] = new Vector2(0, _levelToBuild.size.y);
                            // VirtualCamara
                            var _VirtualCamera = Object.FindObjectOfType<CinemachineVirtualCamera>();
                            _VirtualCamera.transform.position = _Dave.transform.position;
                            _VirtualCamera.Follow = _Dave.transform;
                            // UI
                            UIKit.ClosePanel<UIGameStartPanel>();
                            // TODO: Temp
                            _UIGamePanel = UIKit.OpenPanel<UIGamePanel>();
                            _UIGamePanel.GetComponent<RectTransform>().DOAnchorPosY(200,0);
                        })
                        .DelayFrame(1) // 确保UIKit成功打开UI
                        .Callback(() => // 配置Framework
                        {
                            // 所有数据
                            _LevelModel.OnEnterLevel();
                            // 系统
                            _InputSystem.OnEnterLevel();
                            _HandSystem.OnEnterLevel();
                        })
                        .Callback(() => // 结尾
                        {
                            Time.timeScale = 1;
                            SceneTransitionManager.Display();
                        })
                        .Callback(() => levelState.ChangeState(LevelState.ChoosingCards))
                        .Start(GameManager.Instance);
                });
            //
            levelState.State(LevelState.ChoosingCards)
                .OnEnter(() =>
                {
                    _UILevelChooseCardPanel = UIKit.OpenPanel<UILevelChooseCardPanel>();
                });
            //
            levelState.State(LevelState.Gameplay)
                .OnEnter(() =>
                {
                    ActionKit.Sequence()
                        .Callback(() => // 退出上一个状态
                        {
                            _UILevelChooseCardPanel.Hide();
                            _UILevelChooseCardPanel = null;
                        })
                        .Delay(0.3f) // 等待UI移出摄像机
                        .Callback(() =>
                        {
                            UIKit.ClosePanel<UILevelChooseCardPanel>();
                            //
                            "显示准备安放植物界面".LogInfo();
                        })
                        .Delay(1f)
                        .Callback(() =>
                        {
                        })
                        .DelayFrame(1)
                        .Callback(() =>
                        {
                            _UIGamePanel.Show();
                        })
                        .Delay(1f)
                        .Start(GameManager.Instance);
                });
            levelState.State(LevelState.OutOfLevel);
            //
            levelState.StartState(LevelState.OutOfLevel);
        }

        public void EnterLevel(Level level)
        {
            _levelToBuild = level;
            levelState.ChangeState(LevelState.BuildingLevel);
            //
            cards = new List<Card>();

        }

        public void ExitLevel()
        {
            cards = null;
        }
        #endregion
        
        // ChoosingCard

        private List<Card> cards;
        public bool canAddCard => cards.Count < _LevelModel.maxCards;

        public void AddCard(Card card)
        {
            if (!canAddCard)  return;
            
            cards.Add(card);
        }

        public void RemoveCard(Card card)
        {
            foreach (var eachCard in cards)
            {
                if (ReferenceEquals(eachCard,card))
                {
                    cards.Remove(eachCard);
                    break;
                }
            }
        }
    }
}