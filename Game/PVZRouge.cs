using UnityEngine;
using QFramework;

namespace TPL.PVZR
{
    public class PVZRouge : Architecture<PVZRouge>
    {
        protected override void Init()
        {
            // Model
            
            this.RegisterModel<IGameModel>(new GameModel());
            this.RegisterModel<IDaveModel>(new DaveModel());

            // System
            this.RegisterSystem<InputSystem>(new());
            this.RegisterSystem<IHandSystem>(new HandSystem());
            this.RegisterSystem<IEntityCreateSystem>(new EntityCreateSystem());

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