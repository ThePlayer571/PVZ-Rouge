using UnityEditor;
using UnityEngine;
using System;
using QFramework;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace TPL.PVZR
{
    public static class Global
    {
        public static float peaSpeed = 4f;
        public static float zombieJumpSpeed = 7f;
        public static float peashooterRange = 20f;
        public static float peashooterColdTime = 1.2f;
        public static float sunflowerColdTime = 5f;
        public static float cherryBoomRange = 1f;
        public static float potatoMineSleepTime = 10f;
        public static float potatoMineRange = 0.5f;
        
        public static float Direction2ToFloat(Direction2 dir)
        {
            return dir == Direction2.Right ? 1f : -1f;
        } 

    }
}