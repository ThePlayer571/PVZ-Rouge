using System;
using DG.Tweening;
using QFramework;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Entities.Zombies
{
    public class ZombieViewComponentController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer SpriteRenderer;
        [SerializeField] private Rigidbody2D Rigidbody2D;
        [SerializeField] private Collider2D Collider2D;

        private const float factor = 0.5f;

        public void DisassembleWithForce(Vector2 force)
        {
            Rigidbody2D.simulated = true;
            Collider2D.enabled = true;

            transform.parent = null;
            Rigidbody2D.AddForce(force * factor, ForceMode2D.Impulse);

            ActionKit.Sequence()
                .Delay(2f)
                .Callback(() =>
                {
                    SpriteRenderer.DOFade(0, 2f).OnComplete(() =>
                    {
                        gameObject.DestroySelf();
                    });
                }).Start(this);
        }
    }
}