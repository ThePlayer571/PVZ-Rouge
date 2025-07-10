using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class StorePanelController : MonoBehaviour, IController
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform View;

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}