using System;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class StorePanelController : MonoBehaviour, IController
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform View;

        public void Start()
        {
            toggle.onValueChanged.AddListener(Display);
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(Display);
        }

        private void Display(bool show)
        {
            if (show) View.Show();
            else View.Hide();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}