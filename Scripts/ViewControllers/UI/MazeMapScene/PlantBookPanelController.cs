using System.Collections.Generic;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Helpers.New.GameObjectFactory;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others.UI.MazeMap
{
    public class PlantBookPanelController : MonoBehaviour, IController
    {
        [SerializeField] private RectTransform PlantBookSlots;
        
        private IGameModel _GameModel;
        
        private readonly Dictionary<PlantBookData, GameObject> _plantBookViews = new Dictionary<PlantBookData, GameObject>();
        

        private void Awake()
        {
            _GameModel = this.GetModel<IGameModel>();
        }

        private void Start()
        {
            // 植物秘籍显示 - 初始化
            foreach (var plantBookData in _GameModel.GameData.InventoryData.PlantBooks)
            {
                CreatePlantBookView(plantBookData);
            }

            // 植物秘籍显示 - 变化事件
            _GameModel.GameData.InventoryData.OnPlantBookAdded.Register(CreatePlantBookView)
                .UnRegisterWhenGameObjectDestroyed(this);
        }

        private void CreatePlantBookView(PlantBookData plantBookData)
        {
            var plantBookView = ItemViewFactory.CreateItemView(plantBookData.Id);
            plantBookView.transform.SetParent(PlantBookSlots, false);
            _plantBookViews.Add(plantBookData, plantBookView);
        }
        
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}