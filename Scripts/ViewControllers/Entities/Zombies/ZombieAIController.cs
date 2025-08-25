using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.CommandEvents.New.Level_Shit;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Systems.Level_Data;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies
{
    [Serializable]
    public class ZombieAIController : IController
    {
        private IZombieAISystem _ZombieAISystem;
        private Zombie _attachedZombie;
        private Timer _tryDirtyTimer = new Timer(Global.Zombie_Default_JumpInterval);

        public bool findPathDirty { get; set; } = false;
        public AITendency aiTendency => _attachedZombie.aiTendency;
        public IZombiePath cachePath { get; private set; }
        public MoveData currentMoveData { get; private set; }

        /// <summary>
        /// 寻路的唯一入口，应该由类外调用
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="cellPos"></param>
        /// <returns>是否生成了新的路径</returns>
        public bool TryFindPath(Vector2Int fromPos)
        {
            var targetPos = _ZombieAISystem.PlayerVertexPos;

            bool allowRefind = _ZombieAISystem.ZombieAIUnit.GetVertexSafely(fromPos) != null;
            if (allowRefind && findPathDirty)
            {
                // 寻路
                cachePath = _ZombieAISystem.ZombieAIUnit.FindPath(fromPos, targetPos, aiTendency);
                // 设置数据
                currentMoveData = cachePath.NextTarget();
                findPathDirty = false;
                return true;
            }

            return false;
        }

        public void NextTarget()
        {
            if (cachePath.Count == 0)
            {
                $"cachePath为空时调用NextTarget".LogError();
                return;
            }

            currentMoveData = cachePath.NextTarget();
        }

        /// <summary>
        /// 自动检测并添加dirty
        /// </summary>
        /// <returns>是否为一次有效的检测</returns>
        public bool TryAddDirty()
        {
            var currentCluster = _ZombieAISystem.ZombieAIUnit.GetClusterSafely(_attachedZombie.CellPos);
            if (currentCluster == null || currentMoveData == null)
            {
                return false;
            }

            // 有效检测
            var currentA = currentCluster.vertexA.Position;
            var currentB = currentCluster.vertexB.Position;
            var vertexA = currentMoveData.from;
            var vertexB = currentMoveData.target;
            if (currentA == vertexA || currentA == vertexB || currentB == vertexA || currentB == vertexB)
            {
                // 在正确道路上: do nothing
            }
            else
            {
                findPathDirty = true;
            }

            return true;
        }

        public ZombieAIController(Zombie zombie)
        {
            _attachedZombie = zombie;
            _ZombieAISystem = this.GetSystem<IZombieAISystem>();

            // 自动更新dirty
            this.RegisterEvent<UpdateZombiePathEvent>(e => { findPathDirty = true; })
                .UnRegisterWhenGameObjectDestroyed(_attachedZombie);
            ActionKit.OnUpdate.Register(() =>
            {
                _tryDirtyTimer.Update(Time.deltaTime);
                if (_tryDirtyTimer.Ready)
                {
                    if (TryAddDirty())
                        _tryDirtyTimer.Reset();
                }
            }).UnRegisterWhenGameObjectDestroyed(_attachedZombie);
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}