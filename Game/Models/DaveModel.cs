using UnityEngine;
using QFramework;

namespace TPL.PVZR
{
    public interface IDaveModel:IModel
    {
        public float moveSpeed { get; }
        public Vector2 jumpForce { get; }
        public float jumpSpeed { get; }
    }
    public class DaveModel : AbstractModel, IDaveModel
    {
        public float jumpSpeed => 10f;

        float IDaveModel.moveSpeed => 5f;
        Vector2 IDaveModel.jumpForce => new Vector2(0, 10f);

        protected override void OnInit()
        {
        }
    }
}