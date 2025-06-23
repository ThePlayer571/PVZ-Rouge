using TPL.PVZR.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TPL.PVZR.Helpers.Methods
{
    public static class HandHelper
    {
        public static bool IsHandOnUI() => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();


        public static Vector2Int HandCellPos()
        {
            // 新InputSystem
            var mouseScreenPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();

            var worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            return LevelGridHelper.WorldToCell(new Vector2(worldPos.x, worldPos.y));
        }

        public static bool DaveCanReachHand()
        {
            Vector2 playerPos = ReferenceHelper.Player.transform.position;
            var mousePos = LevelGridHelper.CellToWorld(HandCellPos());

            return Vector2.Distance(playerPos, mousePos) < 5f;
        }
    }
}