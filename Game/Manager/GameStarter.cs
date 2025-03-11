using System.Collections;
using QFramework;
using TPL.PVZR;
using UnityEngine;

namespace TPL.PVZR
{
    public class GameStarter : MonoBehaviour
    {
        void Start()
        {
            ResKit.Init();
            var _ = ResLoader.Allocate();
            var gm  =_.LoadSync<GameObject>("GameManager").Instantiate();
            DontDestroyOnLoad(gm);

            // 初始ui
            UIKit.Root.SetResolution(1920, 1080, 0);
            UIKit.OpenPanel<UIGameStartPanel>();
            // 过渡脚本
        }
    }
}