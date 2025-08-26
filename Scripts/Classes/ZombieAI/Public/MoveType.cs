namespace TPL.PVZR.Classes.ZombieAI.Public
{
    public enum MoveType
    {
        NotSet = 0,
        WalkJump = 1,
        Fall = 2,
        Swim = 3,
        HumanLadder = 4,
        ClimbLadder = 5,
        Climb_WalkJump = 101,
        Climb_Swim = 102,
        Swim_WalkJump = 103,
    }

    public enum MoveStage
    {
        FollowVertex,
        FindDave,
    }
}