using UnityEngine;
using QFramework;
using TPL.PVZR.EntityPlant;

namespace TPL.PVZR
{
    // 游戏进程
    public struct EnterGameSceneInitEvent
    {

    }
    // 输入
    public struct InputJumpEvent{
    }
    public struct InputMoveEvent
    {
        public float speed;
    }
    public struct InputSelectEvent { public Seed seed; }
    public struct InputSelectForceEvent { public Seed seed; }
    public struct InputPickShovelEvent { }
    public struct InputDeselectEvent { }
    public struct InputPlacePlantEvent
    {
        public Direction2 direction;
    }
    public struct InputUseShovelEvent {}

    public struct InputPickSun
    {
        public Sun target;
    }
    // 卡牌相关
    public struct OnSelectSeed
    {
        public Seed seed;
    }
    public struct OnDeselectSeed
    {
        public Seed seed;
    }
    public struct OnPlacePlant
    {
        public Seed seed;
        public Plant plant;
    }
}