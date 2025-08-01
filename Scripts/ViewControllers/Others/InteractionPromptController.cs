using QFramework;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others
{
    public class InteractionPromptController : MonoBehaviour, IController
    {
        private SpriteRenderer _SpriteRenderer;

        private void Awake()
        {
            _SpriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        public void ShowPrompt()
        {
            _SpriteRenderer.enabled = true;
        }

        public void HidePrompt()
        {
            _SpriteRenderer.enabled = false;
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}