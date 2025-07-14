using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Item.Card;
using TPL.PVZR.Classes.DataClasses.Level;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [CreateAssetMenu(fileName = "CardDataListConfig", menuName = "PVZR_Config/CardDataListConfig", order = 6)]
    public class CardDataListConfig: ScriptableObject
    {
        public List<SerializableKeyValuePair<PlantDef, CardDefinition>> cardDict;
    }
}