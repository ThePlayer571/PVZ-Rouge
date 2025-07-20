using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Tools.Save;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Item.PlantBook
{
    public class PlantBookData : ItemData, ISavable<PlantBookSaveData>
    {
        public override ItemType ItemType { get; } = ItemType.PlantBook;

        private PlantBookDefinition Definition { get; }

        public PlantBookData(PlantBookDefinition definition)
        {
            Definition = definition;
        }

        // 便捷属性访问器
        public PlantBookId Id => Definition.Id;
        public Sprite Icon => Definition.Icon;
        public PlantId PlantId => Definition.PlantId;
        public PlantVariant Variant => Definition.Variant;


        public PlantBookData(PlantBookSaveData saveData)
        {
            Definition = PlantBookConfigReader.GetPlantBookDefinition(saveData.plantBookId);
        }

        public PlantBookSaveData ToSaveData()
        {
            return new PlantBookSaveData
            {
                plantBookId = Definition.Id,
            };
        }
    }
}