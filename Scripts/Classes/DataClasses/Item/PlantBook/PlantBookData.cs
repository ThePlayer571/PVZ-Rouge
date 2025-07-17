using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Item.PlantBook
{
    public class PlantBookData : ItemData
    {
        public override ItemType ItemType { get; } = ItemType.PlantBook;
        
        private PlantBookDefinition Definition { get; }

        public PlantBookData(PlantBookDefinition definition)
        {
            Definition = definition;
        }
        
        // 便捷属性访问器
        public Sprite Icon => Definition.Icon;
        public PlantId Id => Definition.Id;
        public PlantVariant Variant => Definition.Variant;
    }
}