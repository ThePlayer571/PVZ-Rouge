using QFramework;

namespace TPL.PVZR.Tools.SoyoFramework
{
    public interface IEvent
    {
    }
    
    /// <summary>
    /// 存在与这个层级紧密相关的Service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasService<out T> where T : IService
    {
        T Service { get; }
    }

    /// <summary>
    /// 该系统维护了数据结构
    /// </summary>
    public interface IDataSystem : ISystem
    {
    }

    /// <summary>
    /// 该系统服务于主进程
    /// </summary>
    public interface IMainSystem : ISystem
    {
    }

    /// <summary>
    /// 该系统会自动更新
    /// </summary>
    public interface IAutoUpdateSystem : ISystem
    {
    }
}