using System;
using FMODUnity;
using QFramework;
using TPL.PVZR.CommandEvents.Level_Gameplay.Coins;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TPL.PVZR.ViewControllers
{
    public class TempTest : MonoBehaviour
    {
        private void Start()
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/TestEvent");
        }
    }
}