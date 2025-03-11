using Cinemachine;
using UnityEngine;
using QFramework;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;

namespace TPL.PVZR
{
    public interface ILevelSystem
    {
        public LevelSystem.LevelState levelState { get; }
    }
    
    
    // 构建关卡/管理关卡进程
    public class LevelSystem : AbstractSystem
    {

        public enum LevelState
        {
            ChoosingCards, Gameplay, End 
        }

        public LevelState levelState { get; private set; } =  LevelState.ChoosingCards;
        private ILevelModel _LevelModel;
        private InputSystem _InputSystem;
        private IHandSystem _HandSystem;
        
        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _InputSystem = this.GetSystem<InputSystem>();
            _HandSystem = this.GetSystem<IHandSystem>();
        }

        public void EnterLevel(Level level)
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
                    _LevelModel.SetLevel(level);
                })
                .Condition(() => ao.isDone) // 成功构建场景
                .Callback(() => // 配置关卡GameObject
                {
                    // Dave
                    var _Dave = _ResLoader.LoadSync<Dave>("Dave")
                        .Instantiate(level.daveInitialPos, Quaternion.identity);
                    // VirtualCamara Confiner
                    var _ConfinerPolygonCollider = GameObject.Find("Confiner").GetComponent<PolygonCollider2D>();
                    _ConfinerPolygonCollider.points[1] = new Vector2(level.size.x, 0);
                    _ConfinerPolygonCollider.points[2] = new Vector2(level.size.x, level.size.y);
                    _ConfinerPolygonCollider.points[3] = new Vector2(0,level.size.y);
                    // VirtualCamara
                    var _VirtualCamera = Object.FindObjectOfType<CinemachineVirtualCamera>();
                    _VirtualCamera.transform.position = _Dave.transform.position;
                    _VirtualCamera.Follow = _Dave.transform;
                    // UI
                    UIKit.ClosePanel<UIGameStartPanel>();
                    UIKit.OpenPanel<UIGamePanel>();
                })
                .DelayFrame(1) // 确保UIKit成功打开UI
                .Callback(() => // 配置Framework
                {
                    // 所有数据
                    _LevelModel.OnEnterLevel();
                    // 系统
                    _InputSystem.OnEnterLevel();
                    _HandSystem.OnEnterLevel();
                    //
                    Time.timeScale = 1;
                    SceneTransitionManager.Display();
                })
                .Start(GameManager.Instance);
        }

    }
}