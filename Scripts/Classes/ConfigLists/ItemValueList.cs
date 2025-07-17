using System.Collections.Generic;
using UnityEngine;

namespace TPL.PVZR.Classes.ConfigLists
{
    [CreateAssetMenu(fileName = "ItemValueList", menuName = "PVZR_Config/ItemValueList")]
    public class ItemValueList : ScriptableObject
    {
        public List<TextAsset> plantValueListJson;
        public List<TextAsset> plantBookValueListJson;
    }
}