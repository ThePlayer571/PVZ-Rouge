using System;
using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Item.PlantBook
{
    [CreateAssetMenu(fileName = "PlantBookDefinition_", menuName = "PVZR/PlantBookDefinition", order = 6)]
    public class PlantBookDefinition : ScriptableObject
    {
        public PlantId Id;
        public PlantVariant Variant;
        public Sprite Icon;
    }
}