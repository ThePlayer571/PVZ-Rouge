using System.Collections.Generic;
using TPL.PVZR.Models;

namespace TPL.PVZR.Events
{
    public struct OnEnterPhaseEarlyEvent
    {
        public GamePhase changeToPhase;
        public Dictionary<string,object> parameters;
    }

    public struct OnEnterPhaseEvent
    {
        public GamePhase changeToPhase;
        public Dictionary<string,object> parameters;
    }

    public struct OnEnterPhaseLateEvent
    {
        public GamePhase changeToPhase;
        public Dictionary<string,object> parameters;
    }

    public struct OnLeavePhaseEarlyEvent
    {
        public GamePhase leaveFromPhase;
        public Dictionary<string,object> parameters;
    }

    public struct OnLeavePhaseEvent
    {
        public GamePhase leaveFromPhase;
        public Dictionary<string,object> parameters;
    }

    public struct OnLeavePhaseLateEvent
    {
        public GamePhase leaveFromPhase;
        public Dictionary<string,object> parameters;
    }
}