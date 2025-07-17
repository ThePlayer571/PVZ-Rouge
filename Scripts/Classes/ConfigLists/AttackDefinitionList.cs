using System;
using System.Collections.Generic;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using UnityEngine;

namespace TPL.PVZR.Classes.ConfigLists
{
    [Serializable]
    public class AttackConfig
    {
        public AttackId attackId;
        public AttackDefinition attackDefinition;
    }
    
    [CreateAssetMenu(fileName = "AttackDefinitionList", menuName = "PVZR_Config/AttackDefinitionList")]
    public class AttackDefinitionList : ScriptableObject
    {
        public List<AttackConfig> attackDefinitionList;
    }
}