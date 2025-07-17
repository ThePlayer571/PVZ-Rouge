using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;
using UnityEngine;

namespace TPL.PVZR.Classes.ConfigLists
{
    [Serializable]
    public class PlantBookConfig
    {
        public PlantBookId plantBookId;
        public PlantBookDefinition plantBookDefinition;
    }

    [CreateAssetMenu(fileName = "PlantBookDefinitionList", menuName = "PVZR_Config/PlantBookDefinitionList")]
    public class PlantBookDefinitionList : ScriptableObject
    {
        public List<PlantBookConfig> plantBookDefinitionList;
    }
}