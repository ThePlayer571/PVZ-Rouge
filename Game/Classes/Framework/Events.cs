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

    public struct WaveStartEvent
    {
        public int wave;
    }
}