using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using TPL.PVZR.Helpers.New.DataReader;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.ViewControllers.Others.UI.ItemView
{
    public class PlantBookViewController : MonoBehaviour
    {
        [SerializeField] private Image plantImage;

        public void Initialize(PlantBookData data)
        {
            plantImage.sprite = data.Icon;
        }

        public void Initialize(PlantBookId plantBookId)
        {
            var definition = PlantBookConfigReader.GetPlantBookDefinition(plantBookId);

            plantImage.sprite = definition.Icon;
        }
    }
}