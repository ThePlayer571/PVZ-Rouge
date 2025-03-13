using UnityEngine;
using QFramework;

namespace TPL.PVZR
{
    public class PVZRouge : Architecture<PVZRouge>
    {
        protected override void Init()
        {
            // Model
            
            this.RegisterModel<ILevelModel>(new LevelModel());
            this.RegisterModel<IDaveModel>(new DaveModel());

            // System
            this.RegisterSystem<IEntityCreateSystem>(new EntityCreateSystem());
            this.RegisterSystem<InventorySystem>(new InventorySystem());
            this.RegisterSystem<GameSystem>(new GameSystem());
            this.RegisterSystem<InputSystem>(new InputSystem());
            // 关卡内
            this.RegisterSystem<IHandSystem>(new HandSystem());
            this.RegisterSystem<LevelSystem>(new LevelSystem());
            this.RegisterSystem<IChooseCardSystem>(new ChooseCardSystem());

            // Utility
            

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