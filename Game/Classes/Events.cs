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
    public struct InputSelectEvent { public Card card; }
    public struct InputSelectForceEvent { public Card card; }
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
    public struct OnSelectCard
    {
        public Card card;
    }
    public struct OnDeselectCard
    {
        public Card card;
    }
    public struct OnPlacePlant
    {
        public Card card;
        public Plant plant;
    }
}