using System;
using TPL.PVZR.Core.Save.Modules;

namespace TPL.PVZR.Core.Save
{
    public interface ISaveModule
    {
        string ModuleKey { get; } // 模块唯一标识
        object GetData(); // 返回数据类
        string Save(); // 返回序列化数据
        void Load(string data); // 反序列化数据
        void Reset(); // 重置为默认状态
    }
}