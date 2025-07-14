using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.DataClasses.Loot;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [CreateAssetMenu(fileName = "ItemValueList", menuName = "PVZR_Config/ItemValueList", order = 5)]
    public class ItemValueList : ScriptableObject
    {
        public List<SerializableKeyValuePair<PlantId, int>> plantValueList;
        public List<SerializableKeyValuePair<PlantBookId, int>> plantBookValueList;
    }
}