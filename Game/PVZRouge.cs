using UnityEngine;
using QFramework;

namespace TPL.PVZR
{
    public class PVZRouge : Architecture<PVZRouge>
    {
        protected override void Init()
        {
            // == Model
            
            this.RegisterModel<ILevelModel>(new LevelModel());
            this.RegisterModel<IDaveModel>(new DaveModel());

            // == System
            // = 整个游戏进程
            this.RegisterSystem<InputSystem>(new InputSystem());
            // = 一局游戏内
            this.RegisterSystem<GameSystem>(new GameSystem());
            this.RegisterSystem<InventorySystem>(new InventorySystem());
            // = 一个关卡内
            this.RegisterSystem<ILevelSystem>(new LevelSystem()); // 管理关卡进程
            this.RegisterSystem<IEntitySystem>(new EntitySystem()); // 负责实体的记录与创建
            this.RegisterSystem<IHandSystem>(new HandSystem()); // 管理手部动作
            this.RegisterSystem<IChooseCardSystem>(new ChooseCardSystem()); // 选择卡片时起作用
            this.RegisterSystem<IWaveSystem>(new WaveSystem()); // 管理记录波次
            this.RegisterSystem<IZombieSpawnSystem>(new ZombieSpawnSystem()); // 管理一波僵尸的生成
            // 关卡内

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