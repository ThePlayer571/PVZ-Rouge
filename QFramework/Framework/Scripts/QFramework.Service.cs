/****************************************************************************
 * QFramework Service Extension
 * Service层级扩展，提供游戏操作的唯一入口
 ****************************************************************************/

using System.Linq;

namespace QFramework
{
    #region Service Interface & Architecture Extension

    /// <summary>
    /// Service接口 - 游戏操作的唯一入口
    /// </summary>
    public interface IService : ICanInit, ICanGetModel, ICanGetSystem, ICanSendEvent, IBelongToArchitecture,
        ICanSetArchitecture, ICanGetService
    {
    }

    /// <summary>
    /// Service抽象基类
    /// </summary>
    public abstract class AbstractService : IService
    {
        private IArchitecture mArchitecture;

        IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;

        public bool Initialized { get; set; }
        void ICanInit.Init() => OnInit();

        public void Deinit() => OnDeinit();

        protected virtual void OnDeinit()
        {
        }

        protected abstract void OnInit();
    }

    /// <summary>
    /// IArchitecture的Service扩展
    /// </summary>
    public partial interface IArchitecture
    {
        void RegisterService<T>(T service) where T : IService;
        T GetService<T>() where T : class, IService;
    }

    /// <summary>
    /// Architecture的Service扩展实现
    /// </summary>
    public abstract partial class Architecture<T>
    {
        public void RegisterService<TService>(TService service) where TService : IService
        {
            service.SetArchitecture(this);
            mContainer.Register<TService>(service);

            if (mInited)
            {
                service.Init();
                service.Initialized = true;
            }
        }

        public TService GetService<TService>() where TService : class, IService => mContainer.Get<TService>();
    }

    #endregion

    #region Can Get Service Interface

    /// <summary>
    /// 可以获取Service的接口
    /// </summary>
    public interface ICanGetService : IBelongToArchitecture
    {
    }

    /// <summary>
    /// 获取Service的扩展方法
    /// </summary>
    public static class CanGetServiceExtension
    {
        public static T GetService<T>(this ICanGetService self) where T : class, IService =>
            self.GetArchitecture().GetService<T>();
    }

    #endregion
}