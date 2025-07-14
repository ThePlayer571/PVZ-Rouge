using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class CoinStorePanelController : MonoBehaviour, IController
    {
        [SerializeField] private Toggle mainToggle;
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform View;


        private IStoreSystem _StoreSystem;
        private IGameModel _GameModel;

        private void Awake()
        {
            _StoreSystem = this.GetSystem<IStoreSystem>();
            _GameModel = this.GetModel<IGameModel>();
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(Display);
            mainToggle.onValueChanged.AddListener(Display);
        }

        private void Display(bool val)
        {
            if (mainToggle.isOn && toggle.isOn) View.Show();
            else View.Hide();
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}