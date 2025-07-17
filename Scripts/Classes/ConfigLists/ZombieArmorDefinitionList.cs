using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses_InLevel.ZombieArmor;
using UnityEngine;

namespace TPL.PVZR.Classes.ConfigLists
{
    [Serializable]
    public class ZombieArmorConfig
    {
        public ZombieArmorId zombieArmorId;
        public ZombieArmorDefinition zombieArmorDefinition;
    }

    [CreateAssetMenu(fileName = "ZombieArmorDefinitionList", menuName = "PVZR_Config/ZombieArmorDefinitionList")]
    public class ZombieArmorDefinitionList : ScriptableObject
    {
        public List<ZombieArmorConfig> zombieArmorDefinitionList;
    }
}