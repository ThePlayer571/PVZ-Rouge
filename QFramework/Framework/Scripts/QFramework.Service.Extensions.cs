/****************************************************************************
 * QFramework Service Interface Extensions
 * 为现有接口添加获取Service的能力
 ****************************************************************************/

namespace QFramework
{
    public partial interface IController : ICanGetService
    {
    }

    public partial interface ICommand : ICanGetService
    {
    }

    public partial interface ICommand<TResult> : ICanGetService
    {
    }

    public partial interface ISystem : ICanGetService
    {
    }
}