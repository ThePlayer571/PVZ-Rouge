using System.Collections.Generic;
using TPL.PVZR.Models;

namespace TPL.PVZR.CommandEvents.Phase
{
    public enum PhaseStage
    {
        EnterEarly,
        EnterNormal,
        EnterLate,
        LeaveEarly,
        LeaveNormal,
        LeaveLate
    }

    public struct OnPhaseChangeEvent
    {
        public GamePhase GamePhase;
        public PhaseStage PhaseStage;
        public Dictionary<string, object> Parameters;
    }
}