using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses.Attack;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Recipe
{
    [Serializable]
    public class AttackConfig
    {
        public AttackId attackId;
        public AttackDefinition attackDefinition;
    }
    
    public class AttackDefinitionList : ScriptableObject
    {
        public List<AttackConfig> attackDefinitionList;
    }
}