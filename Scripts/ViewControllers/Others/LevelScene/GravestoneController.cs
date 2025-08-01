using System;
using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Systems.Level_Event;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others.LevelScene
{
    public class GravestoneController : MonoBehaviour, IController
    {
        public static List<GravestoneController> Instances { get; } = new();
        
        private void Awake()
        {
            Instances.Add(this);
        }

        private void OnDestroy()
        {
            Instances.Remove(this);
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}