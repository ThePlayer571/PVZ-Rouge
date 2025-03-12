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
            this.RegisterSystem<InputSystem>(new InputSystem());
            this.RegisterSystem<IHandSystem>(new HandSystem());
            this.RegisterSystem<IEntityCreateSystem>(new EntityCreateSystem());
            this.RegisterSystem<LevelSystem>(new LevelSystem());
            this.RegisterSystem<InventorySystem>(new InventorySystem());
            this.RegisterSystem<GameSystem>(new GameSystem());

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