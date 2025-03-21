using QFramework;

namespace TPL.PVZR
{
    public interface ILevelSystem:ISystem
    {
        void EnterLevel(ILevel level);
        void TryEndLevel();
        public FSM<LevelSystem.LevelState> levelState { get; }
    }


    // 构建关卡/管理关卡进程
    public partial class LevelSystem : AbstractSystem,ILevelSystem
    {
        # region 通用

        public enum LevelState
        {
            OutOfLevel,
            BuildingLevel,
            ChoosingCards,
            Gameplay,
            EndGameplay,
            ChooseLoots,
            Defeat,
            Exiting
        }

        public FSM<LevelState> levelState { get; private set; }
        private ILevelModel _LevelModel;
        private InputSystem _InputSystem;
        private IHandSystem _HandSystem;
        private IChooseCardSystem _ChooseCardSystem;
        private IWaveSystem _WaveSystem;
        private IEntitySystem _EntitySystem;
        private IZombieSpawnSystem _ZombieSpawnSystem;
        private IInventorySystem _InventorySystem;
        
        // 为方便调用而存的变量
        private UILevelChooseCardPanel _UILevelChooseCardPanel;
        private UILevelPanel _UILevelPanel;
        
        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            _InputSystem = this.GetSystem<InputSystem>();
            _HandSystem = this.GetSystem<IHandSystem>();
            _ChooseCardSystem = this.GetSystem<IChooseCardSystem>();
            _WaveSystem = this.GetSystem<IWaveSystem>();
            _EntitySystem = this.GetSystem<IEntitySystem>();
            _ZombieSpawnSystem = this.GetSystem<IZombieSpawnSystem>();
            _InventorySystem = this.GetSystem<IInventorySystem>();
            //
            levelState = new FSM<LevelState>();
            //
            SetUpState();
        }

        private ILevel _levelToBuild;


        public void EnterLevel(ILevel level)
        {
            _levelToBuild = level;
            levelState.ChangeState(LevelState.BuildingLevel);

        }
        #endregion
        public void TryEndLevel()
        // 调用后，尝试结束游戏，根据当前状态判断是否应该结束
        {
            if (_WaveSystem.currentWave == _LevelModel.WaveConfig.totalWaveCount && _EntitySystem.ZombieSet.Count == 0 &&
                _ZombieSpawnSystem.activeZombieSpawners.Count == 0)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                levelState.ChangeState(LevelState.EndGameplay);
            }
        }
        
    }
}