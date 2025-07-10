using System;
using QFramework;
using TPL.PVZR.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class RecipeStorePanelController : MonoBehaviour, IController
    {
        [SerializeField] private Toggle mainToggle;
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform View;
        
        [SerializeField] private RectTransform Trades;
        [SerializeField] private GameObject TradePrefab;

        private IStoreSystem _StoreSystem;

        private void Awake()
        {
            _StoreSystem = this.GetSystem<IStoreSystem>();
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener(Display);
            mainToggle.onValueChanged.AddListener(Display);
            
            // 创建Trades
            foreach (var recipeData in _StoreSystem.ActiveRecipes)
            {
                // 创建Trade节点
                var trade = TradePrefab.Instantiate().GetComponent<RecipeTradeNode>();
                trade.transform.SetParent(Trades, false);
                // 填充Ingredients
                
                
            }
            
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(Display);
            mainToggle.onValueChanged.RemoveListener(Display);
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