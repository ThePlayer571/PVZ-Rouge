using System;
using System.Collections.Generic;

namespace TPL.PVZR.Core.Extensions
{
    public static class DictionaryExtensions
    {
        // 严格版本：字典为 null 或参数缺失时抛出异常
        public static T GetPara<T>(this Dictionary<string, object> parameters, string key)
        {
            // 严格模式下检查字典和键的合法性
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters), "参数字典不能为 null");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("键不能为空或空字符串", nameof(key));

            // 获取并验证参数类型
            if (!(parameters.TryGetValue(key, out object valObject) && valObject is T val))
                throw new ArgumentException($"参数缺失或类型不匹配：{key}（期望类型 {typeof(T)}）");

            return val;
        }

        // 宽松版本：字典为 null 或参数缺失时返回默认值
        public static T GetPara<T>(this Dictionary<string, object> parameters, string key, T defaultValue)
        {
            // 如果字典为 null，直接返回默认值（不再验证键合法性）
            if (parameters == null)
                return defaultValue;

            // 如果键不合法，仍然抛出异常（防止无效键滥用）
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("键不能为空或空字符串", nameof(key));

            // 获取值或返回默认值
            return parameters.TryGetValue(key, out object valObject) && valObject is T val 
                ? val 
                : defaultValue;
        }
    }
}