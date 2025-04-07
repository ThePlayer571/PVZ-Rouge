using UnityEngine;
using UnityEngine.Serialization;

namespace TPL.PVZR.Gameplay.Data
{
    [CreateAssetMenu(fileName = "CardData_", menuName = "PVZR/CardData", order = 0)]
    public class CardSO:ScriptableObject
    {
        public SeedSO seedSO;
    }
}