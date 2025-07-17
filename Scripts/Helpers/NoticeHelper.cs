using QFramework;

namespace TPL.PVZR.Helpers.New
{
    /// <summary>
    /// 我想发出音效/提示，但是还没做，就用这个代替，方便未来设置（看那些类用了这个就行）
    /// </summary>
    public static class NoticeHelper
    {
        public static void Notice(string msg) => msg.LogInfo();
    }
}