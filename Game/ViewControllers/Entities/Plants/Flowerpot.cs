namespace TPL.PVZR.EntityPlant
{
    public class Flowerpot:Plant
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Dead()
        {
            var upCell = _LevelModel.CellGrid[gridPos2.x, gridPos2.y + 1];
            if (upCell.HavePlant)
            {
                upCell.plant.Kill();
            }
            base.Dead();
        }
    }
}