using QFramework;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems;
using TPL.PVZR.Architecture.Systems.InGame;
using TPL.PVZR.Architecture.Systems.InLevel;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Core;
using TPL.PVZR.Core.Save;

namespace TPL.PVZR.Architecture
{
    public class PVZRouge : Architecture<PVZRouge>
    {
        protected override void Init()
        {
            // == Model
            this.RegisterModel<IGameModel>(new GameModel());
            this.RegisterModel<ILevelModel>(new LevelModel());

            // == System
            // PhaseSystem
            this.RegisterSystem<IGamePhaseSystem>(new GamePhaseSystem());
            this.RegisterSystem<IMainGameSystem>(new MainGameSystem());
            this.RegisterSystem<IGameSystem>(new GameSystem());
            this.RegisterSystem<ILevelSystem>(new LevelSystem()); // 管理关卡进程
            // 全局
            this.RegisterSystem(new SaveSystem());
            this.RegisterSystem<InputSystem>(new InputSystem());
            // 一局游戏内
            this.RegisterSystem<IInventorySystem>(new InventorySystem());
            this.RegisterSystem<ILootCreateSystem>(new LootCreateSystem());
            // 一个关卡内
            this.RegisterSystem<IEntitySystem>(new EntitySystem()); // 负责实体的记录与创建
            this.RegisterSystem<IHandSystem>(new HandSystem()); // 管理手部动作
            this.RegisterSystem<IChooseCardSystem>(new ChooseCardSystem()); // 选择卡片时起作用
            this.RegisterSystem<IWaveSystem>(new WaveSystem()); // 管理记录波次
            this.RegisterSystem<IZombieSpawnSystem>(new ZombieSpawnSystem()); // 管理一波僵尸的生成

            // == Utility
            

        }
    }

}

// == Controller
// ��ܽӿ�
// Model|System
// ����
// ����
// ����
// ��ʼ��
// == �߼�
// ����
// ����