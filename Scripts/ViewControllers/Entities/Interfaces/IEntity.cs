using QFramework;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Interfaces
{
    public interface IEntity : IController
    {
        Vector2Int CellPos { get; }
        void Remove();
    }
}