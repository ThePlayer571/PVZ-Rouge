using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.ZombieArmor;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [Serializable]
    public class ZombieArmorConfig
    {
        public ZombieArmorId zombieArmorId;
        public ZombieArmorDefinition zombieArmorDefinition;
    }
    
    public class ZombieArmorDefinitionList : ScriptableObject
    {
        public List<ZombieArmorConfig> zombieArmorDefinitionList;
    }
}