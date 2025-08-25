using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using TPL.PVZR.ViewControllers.Entities.Zombies.States;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies.Instances
{
    public sealed class DiggerZombie : Zombie
    {
        public override ZombieId Id { get; } = ZombieId.DiggerZombie;

        [NonSerialized] public float _excavatePosX;
        [SerializeField] public Collider2D _collider;

        public override void OnInit(IList<string> paras)
        {
            baseAttackData = AttackCreator.CreateAttackData(AttackId.NormalZombie);
            Health.Value = GlobalEntityData.Zombie_Default_Health;

            // 解析出土x坐标字符串
            _excavatePosX = ParseExcavateXPosition(paras.FirstOrDefault());
        }

        public override float GetSpeed()
        {
            if (FSM.CurrentStateId == ZombieState.Digging_DiggerZombie)
            {
                return base.GetSpeed() * GlobalEntityData.Zombie_DiggerZombie_SpeedMultiplier;
            }
            else
            {
                return base.GetSpeed();
            }
        }


        protected override void SetUpFSM()
        {
            FSM.AddState(ZombieState.Digging_DiggerZombie, new DiggingState_DiggerZombie(FSM, this));
            FSM.AddState(ZombieState.Excavating_DiggerZombie, new ExcavatingState_DiggerZombie(FSM, this));
            FSM.AddState(ZombieState.DefaultTargeting, new DefaultTargetingState(FSM, this));
            FSM.AddState(ZombieState.Attacking, new AttackingState(FSM, this));
            FSM.AddState(ZombieState.Stunned, new StunnedState(FSM, this));
            FSM.AddState(ZombieState.Dead, new DeadState(FSM, this));

            FSM.StartState(ZombieState.Digging_DiggerZombie);
        }

        #region 函数

        /// <summary>
        /// 解析出土x坐标字符串为float，异常处理和注释均为中文
        /// </summary>
        /// <param name="xStr">x坐标字符串</param>
        /// <returns>解析后的x坐标，异常时返回0</returns>
        [Pure]
        private float ParseExcavateXPosition(string xStr)
        {
            if (string.IsNullOrEmpty(xStr))
            {
                Debug.LogError("DiggerZombie: 出土x坐标字符串为空或null。");
                return 0f;
            }

            try
            {
                float x;
                // 直接尝试解析为浮点数
                if (!float.TryParse(xStr, out x))
                {
                    Debug.LogError($"DiggerZombie: 出土x坐标字符串无法解析为数字，原始值: {xStr}");
                    return 0f;
                }

                return x;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"DiggerZombie: 解析出土x坐标 '{xStr}' 失败。异常信息: {ex.Message}");
                return 0f;
            }
        }

        #endregion
    }
}