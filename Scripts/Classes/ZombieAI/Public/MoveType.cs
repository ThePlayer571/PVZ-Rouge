namespace TPL.PVZR.Classes.ZombieAI.Public
{
    public enum MoveType
    {
        NotSet,
        WalkJump,
        Fall,
        Swim,
        HumanLadder,
        ClimbLadder,
        Climb_WalkJump,
        Climb_Swim,
        Swim_WalkJump,
    }

    public enum MoveStage
    {
        FollowVertex,
        FindDave,
    }
}