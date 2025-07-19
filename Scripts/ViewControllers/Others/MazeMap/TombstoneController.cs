using QFramework;
using TPL.PVZR.Classes.DataClasses.Tomb;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.Systems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPL.PVZR.ViewControllers.Others.MazeMap
{
    public class TombstoneController : MonoBehaviour, IController, IPointerClickHandler
    {
        [SerializeField] private GameObject Tomb;
        [SerializeField] private GameObject TombCracked;
        [SerializeField] private GameObject TombDestroyed;
        [SerializeField] private GameObject TombDark;


        private ITombData _tombData;
        private bool _isActive = false;

        public void Initialize(Vector2Int position)
        {
            var controller = this.GetSystem<IMazeMapSystem>()._MazeMapController;
            var _ = controller.MatrixToTilemapPosition(position);
            transform.position = MazeMapTilemapController.Instance.Ground.CellToWorld(new Vector3Int(_.x, _.y, 0));


            var state = controller.GetTombState(position);
            switch (state)
            {
                case TombState.Active:
                    Tomb.Show();
                    _isActive = true;
                    break;
                case TombState.Passed or TombState.Current: TombDestroyed.Show(); break;
                case TombState.FormlyDiscovered: TombCracked.Show(); break;
                case TombState.NotDiscovered: TombDark.Show(); break;
            }

            _tombData = controller.GetTomb(position);
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isActive)
                this.SendCommand<StartLevelCommand>(new StartLevelCommand(_tombData));
        }
    }
}