using QFramework;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPL.PVZR.ViewControllers.Others.MazeMap
{
    public class TempEndGameObjectController : MonoBehaviour, IController, IPointerClickHandler
    {
        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var gamePhaseChangeService = this.GetService<IGamePhaseChangeService>();
            gamePhaseChangeService.ExitGameWithGamePassed();
        }
    }
}