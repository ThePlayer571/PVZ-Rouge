using System;
using UnityEngine;

namespace TPL.PVZR.Classes.Others
{
    [CreateAssetMenu(fileName = "Tip_", menuName = "PVZR/Tip", order = 2)]
    public class TipDefinition : ScriptableObject
    {
        public TipId Id;
        public string Title;
        public string Body;
    }

    [Serializable]
    public enum TipId
    {
        NotSet = 0,
        FirstEnterGame = 1,
    }
}