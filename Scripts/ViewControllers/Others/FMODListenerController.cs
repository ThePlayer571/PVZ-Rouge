using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using TPL.PVZR.Tools;

namespace TPL.PVZR.ViewControllers.Others
{
    [RequireComponent(typeof(StudioListener))]
    public class ListenerController : MonoBehaviour
    {
        [Header("监听器设置")] [SerializeField, Range(0, 100)]
        private int priority = 50;

        private FMODUnity.StudioListener fmodListener;

        // 静态管理
        private static List<ListenerController> allListeners = new List<ListenerController>();

        public int Priority
        {
            get => priority;
            set
            {
                priority = value;
                RefreshListeners();
            }
        }

        void Awake()
        {
            fmodListener = GetComponent<FMODUnity.StudioListener>();
            if (fmodListener == null)
            {
                Debug.LogError($"ListenerController needs FMODUnity.StudioListener component on {gameObject.name}");
                enabled = false;
                return;
            }

            fmodListener.enabled = false;
        }

        void Start()
        {
            allListeners.Add(this);
            RefreshListeners();
        }

        void OnDestroy()
        {
            allListeners.Remove(this);
            RefreshListeners();
        }

        private static void RefreshListeners()
        {
            // 清理无效监听器
            allListeners.RemoveAll(l => l == null);

            // 先全部禁用
            foreach (var listener in allListeners)
                listener.fmodListener.enabled = false;

            // 启用优先级最高的
            if (allListeners.Any())
            {
                var activeListener = allListeners
                    .Where(l => l.enabled && l.gameObject.activeInHierarchy)
                    .MaxBy(l => l.Priority);
                activeListener.fmodListener.enabled = true;
            }
        }
    }
}