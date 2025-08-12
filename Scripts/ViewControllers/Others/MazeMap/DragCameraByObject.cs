using Cinemachine;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPL.PVZR.ViewControllers.Others.MazeMap
{
    public class DragCameraByObject : MonoBehaviour, IDragHandler
    {
        public CinemachineVirtualCamera virtualCamera;
        public Transform cameraTarget;

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 deltaScreen = new Vector3(-eventData.delta.x, -eventData.delta.y, 0);
            Vector3 deltaWorld = Camera.main.ScreenToWorldPoint(deltaScreen)
                                 - Camera.main.ScreenToWorldPoint(Vector3.zero);

            cameraTarget.position += deltaWorld;
        }
    }
}