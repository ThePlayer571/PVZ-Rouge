using QFramework;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Managers
{
    public class TestDataManager :MonoSingleton<TestDataManager>
    {
        [SerializeField] public float Power = 1f;
        [SerializeField] public float StartPosOffset = 0.5f;
        
    }
}