using QFramework;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities
{
    public interface IEntity : IController
    {
        Vector2Int CellPos { get; }
    }
}