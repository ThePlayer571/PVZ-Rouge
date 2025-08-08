using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace TPL.PVZR.Services
{
    public interface IUIStackService : IService
    {
        bool HasPanelInStack();
        void PushPanel(ICanPutInUIStack uiPanel);
        void PopIfTop(ICanPutInUIStack uiPanel);
        void PopTop();
        void Clear();
    }

    public interface ICanPutInUIStack
    {
        void OnPopped();
    }

    public class UIStackService : AbstractService, IUIStackService
    {
        private Stack<ICanPutInUIStack> _stack = new();

        protected override void OnInit()
        {
        }

        public bool HasPanelInStack()
        {
            ClearTopNulls();
            return _stack.Count > 0;
        }

        private void ClearTopNulls()
        {
            while (_stack.Count > 0)
            {
                var top = _stack.Peek();
                if (top == null || top is MonoBehaviour monoBehaviour && monoBehaviour == null)
                {
                    // todo 暂时保留方便查看报错信息，以后删除了也无所谓
                    $"发现错误：UIStackService 栈顶有 null，已自动清理".LogError();
                    _stack.Pop();
                }
                else return;
            }
        }

        public void PushPanel(ICanPutInUIStack uiPanel)
        {
            if (_stack.Count > 5) $"_stack.Count > 5".LogWarning();
            _stack.Push(uiPanel);
        }

        public void PopIfTop(ICanPutInUIStack uiPanel)
        {
            ClearTopNulls();
            if (_stack.Count > 0 && _stack.Peek() == uiPanel)
            {
                var top = _stack.Pop();
                top.OnPopped();
            }
        }

        public void PopTop()
        {
            ClearTopNulls();
            if (_stack.Count > 0)
            {
                var top = _stack.Pop();
                top.OnPopped();
            }
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }
}