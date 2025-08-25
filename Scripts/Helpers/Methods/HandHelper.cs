using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers;
using TPL.PVZR.ViewControllers.Managers;
using UnityEngine;

namespace TPL.PVZR.Helpers.New.Methods
{
    public static class HandHelper
    {
        public static bool IsHandOnUI() => GameManager.IsPointerOverUI;


        public static Vector2Int HandCellPos()
        {
            // 新InputSystem
            var mouseScreenPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            // todo 优化Camera.main
            var worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            return LevelGridHelper.WorldToCell(new Vector2(worldPos.x, worldPos.y));
        }

        public static bool DaveCanReachHand()
        {
            Vector2 playerPos = Player.Instance.transform.position;
            var mousePos = LevelGridHelper.CellToWorld(HandCellPos());

            return Vector2.Distance(playerPos, mousePos) < 5f;
        }

        public static bool DaveCanReach(Vector2Int cellPos)
        {
            Vector2 playerPos = Player.Instance.transform.position;
            var worldPos = LevelGridHelper.CellToWorld(cellPos);

            return Vector2.Distance(playerPos, worldPos) < 5f;
        }

        public static bool DaveCanReach(Vector2 worldPos)
        {
            Vector2 playerPos = Player.Instance.transform.position;

            return Vector2.Distance(playerPos, worldPos) < 5f;
        }
    }
}