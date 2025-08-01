using System;
using UnityEngine;

namespace PVZR.Tools
{
    /// <summary>
    /// 枚举分页显示属性，类似Odin Inspector的EnumPaging功能
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumPagingAttribute : PropertyAttribute
    {
        /// <summary>
        /// 每页显示的枚举项数量
        /// </summary>
        public int ItemsPerPage { get; }
        
        /// <summary>
        /// 是否显示搜索框
        /// </summary>
        public bool ShowSearchBar { get; }
        
        /// <summary>
        /// 是否显示枚举值
        /// </summary>
        public bool ShowEnumValues { get; }

        public EnumPagingAttribute(int itemsPerPage = 10, bool showSearchBar = true, bool showEnumValues = false)
        {
            ItemsPerPage = itemsPerPage;
            ShowSearchBar = showSearchBar;
            ShowEnumValues = showEnumValues;
        }
    }
}
