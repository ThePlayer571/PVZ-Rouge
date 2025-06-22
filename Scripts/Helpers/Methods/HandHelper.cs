using TPL.PVZR.Core;
using UnityEngine;

namespace TPL.PVZR.Helpers.Methods
{
    public static class HandHelper
    {
        public static Vector2Int MouseCellPos()
        {
            // 新InputSystem
            var mouseScreenPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();

            var worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            return LevelTilemapHelper.WorldToCell(new Vector2(worldPos.x, worldPos.y));
        }

        public static bool PlayerCanReachMouse()
        {
            Vector2 playerPos = ReferenceHelper.Player.transform.position;
            var mousePos = LevelTilemapHelper.CellToWorld(MouseCellPos());

            return Vector2.Distance(playerPos, mousePos) < 5f;
        }
    }
}