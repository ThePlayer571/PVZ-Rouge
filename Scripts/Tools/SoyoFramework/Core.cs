using QFramework;

namespace TPL.PVZR.Tools.SoyoFramework
{
    public interface IEvent
    {
    }

    /// <summary>
    /// 该事件作为某服务的唯一入口（只允许单个接收者）
    /// </summary>
    public interface IServiceEvent : IEvent
    {
    }

    /// <summary>
    /// 该系统只用于处理服务相关的事件
    /// </summary>
    public interface IServiceManageSystem : ISystem
    {
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