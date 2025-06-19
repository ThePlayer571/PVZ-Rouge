using System;
using TPL.PVZR.Classes.LevelStuff;

namespace TPL.PVZR.ViewControllers.Entities.Zombies
{
    public abstract class Zombie:Entity, IEffectable, IAttackable
    {
        #region Effect系统

        protected EffectGroup effectGroup;
        
        
        public void TakeEffect(Effect effect)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region 血量系统

        protected float health;
        
        public void TakeAttack()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region 寻路系统

        

        #endregion

        protected float speed;

    }
}