using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses.Level
{
    [CreateAssetMenu(fileName = "LevelDefinition_", menuName = "PVZR/LevelDefinition", order = 2)]
    public class LevelDefinition : ScriptableObject
    {
        public LevelId LevelId;
        public Vector2Int MapSize;
        public Vector2 InitialPlayerPos;
        public GameObject LevelPrefab;
    }
}