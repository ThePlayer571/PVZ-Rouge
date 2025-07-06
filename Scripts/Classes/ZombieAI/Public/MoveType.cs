namespace TPL.PVZR.Classes.ZombieAI.Public
{
    public enum MoveType
    {
        NotSet,
        WalkJump,
        Fall,
        Water,
        HumanLadder,
        ClimbLadder,
        ClimbWalkJump,
    }

    public enum MoveStage
    {
        FollowVertex,
        FindDave,
    }
}