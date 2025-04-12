using System.Collections.Generic;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Core;

namespace TPL.PVZR.Architecture.Events
{
    // 输入事件
    namespace Input
    {
        public struct InputJumpEvent
        {
        }

        public struct InputMoveEvent
        {
            public float speed;
        }

        public struct InputSelectEvent
        {
            public int seedIndex;
        }

        public struct InputSelectForceEvent
        {
            public int seedIndex;
        }

        public struct InputPickShovelEvent
        {
        }

        public struct InputDeselectEvent
        {
        }

        public struct InputPlacePlantEvent
        {
            public Direction2 direction;
        }

        public struct InputUseShovelEvent
        {
        }

        public struct InputInteractEvent
        {
        }
    }

    // 游戏进程
    namespace GamePhase // 目前只用于System初始化
    {
        public struct OnEnterPhaseEarlyEvent
        {
            public GamePhaseSystem.GamePhase changeToPhase;
            public Dictionary<string,object> parameters;
        }

        public struct OnEnterPhaseEvent
        {
            public GamePhaseSystem.GamePhase changeToPhase;
            public Dictionary<string,object> parameters;
        }

        public struct OnEnterPhaseLateEvent
        {
            public GamePhaseSystem.GamePhase changeToPhase;
            public Dictionary<string,object> parameters;
        }

        public struct OnLeavePhaseEarlyEvent
        {
            public GamePhaseSystem.GamePhase leaveFromPhase;
            public Dictionary<string,object> parameters;
        }

        public struct OnLeavePhaseEvent
        {
            public GamePhaseSystem.GamePhase leaveFromPhase;
            public Dictionary<string,object> parameters;
        }

        public struct OnLeavePhaseLateEvent
        {
            public GamePhaseSystem.GamePhase leaveFromPhase;
            public Dictionary<string,object> parameters;
        }
    }

    // 存储
    namespace Save
    {
        public struct OnSaveBegin
        {
        };

        public struct OnSaveComplete
        {
        };

        public struct OnLoadBegin
        {
        };

        public struct OnLoadComplete
        {
        };
    }

    public struct EnterGameSceneInitEvent
    {
    }

    // 输入
    public struct WaveStartEvent
    {
        public int wave;
    }

    // Gameplay
    public struct OnZombieDestroyedEvent
    {
    }
}