/*
 * =============================================================================
 * EnumPaging 自定义属性 - 使用教程
 * =============================================================================
 * 
 * 这是一个免费的枚举分页显示解决方案，可以替代Odin Inspector的付费EnumPaging功能
 * 
 * 📖 基本用法:
 * 在枚举字段上添加 [EnumPaging] 属性即可：
 * 
 *     [EnumPaging]
 *     public PlantId selectedPlant;
 * 
 * 📖 高级配置:
 * 
 * 1. 自定义每页显示数量 (默认10个)：
 *     [EnumPaging(itemsPerPage: 5)]
 *     public PlantId plant;
 * 
 * 2. 关闭搜索功能 (默认开启)：
 *     [EnumPaging(showSearchBar: false)]
 *     public PlantId plant;
 * 
 * 3. 显示枚举数值 (默认不显示)：
 *     [EnumPaging(showEnumValues: true)]
 *     public PlantId plant;  // 显示为: "Peashooter (0)"
 * 
 * 4. 组合配置：
 *     [EnumPaging(itemsPerPage: 8, showSearchBar: true, showEnumValues: true)]
 *     public PlantId plant;
 * 
 * 📖 支持的数据类型:
 * - 普通枚举字段
 * - 枚举数组
 * - List<枚举> (需要在List的泛型类型是枚举)
 * 
 * 📖 功能特性:
 * ✅ 分页显示 - 避免长枚举列表
 * ✅ 实时搜索 - 快速定位目标枚举
 * ✅ 分页导航 - 上一页/下一页按钮
 * ✅ 下拉界面 - 点击展开/收起
 * ✅ 高亮选中 - 当前选中项有背景色
 * ✅ 完全免费 - 无需付费插件
 * 
 * 📖 使用示例:
 * 
 *     public class MyScript : MonoBehaviour 
 *     {
 *         [Header("基础用法")]
 *         [EnumPaging]
 *         public PlantId mainPlant;
 *         
 *         [Header("小页面模式")]
 *         [EnumPaging(itemsPerPage: 5)]
 *         public PlantId smallPagePlant;
 *         
 *         [Header("显示数值模式")]
 *         [EnumPaging(showEnumValues: true)]
 *         public PlantId plantWithValues;
 *         
 *         [Header("数组支持")]
 *         [EnumPaging(itemsPerPage: 6)]
 *         public PlantId[] plantArray = new PlantId[3];
 *     }
 * 
 * 📖 操作说明:
 * - 点击枚举字段可展开/收起选择器
 * - 在搜索框中输入关键词过滤枚举项
 * - 使用 ◄ ► 按钮在页面间导航
 * - 点击枚举项选择并自动关闭选择器
 * - 点击选择器外部区域关闭选择器
 * 
 * 📖 注意事项:
 * - 仅支持枚举类型，其他类型会显示错误信息
 * - 搜索功能不区分大小写
 * - 每个枚举字段的页面状态独立保存
 * 
 * =============================================================================
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using PVZR.Tools;

[CustomPropertyDrawer(typeof(EnumPagingAttribute))]
public class EnumPagingDrawer : PropertyDrawer
{
    private static Dictionary<string, int> _currentPages = new Dictionary<string, int>();
    private static Dictionary<string, string> _searchTexts = new Dictionary<string, string>();
    private static Dictionary<string, bool> _isDropdownOpen = new Dictionary<string, bool>();
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Enum)
            return EditorGUIUtility.singleLineHeight;
            
        var attribute = (EnumPagingAttribute)this.attribute;
        string propertyPath = property.propertyPath;
        
        if (!_isDropdownOpen.ContainsKey(propertyPath))
            _isDropdownOpen[propertyPath] = false;
            
        if (!_isDropdownOpen[propertyPath])
            return EditorGUIUtility.singleLineHeight;
            
        // 计算下拉框展开时的高度
        float height = EditorGUIUtility.singleLineHeight; // 标题行
        
        if (attribute.ShowSearchBar)
            height += EditorGUIUtility.singleLineHeight + 2; // 搜索框
            
        height += EditorGUIUtility.singleLineHeight + 2; // 分页控件
        height += attribute.ItemsPerPage * EditorGUIUtility.singleLineHeight; // 枚举项
        height += 4; // 间距
        
        return height;
    }
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Enum)
        {
            EditorGUI.LabelField(position, label.text, "EnumPaging只能用于枚举类型");
            return;
        }
        
        var attribute = (EnumPagingAttribute)this.attribute;
        string propertyPath = property.propertyPath;
        
        // 初始化字典
        if (!_currentPages.ContainsKey(propertyPath))
            _currentPages[propertyPath] = 0;
        if (!_searchTexts.ContainsKey(propertyPath))
            _searchTexts[propertyPath] = "";
        if (!_isDropdownOpen.ContainsKey(propertyPath))
            _isDropdownOpen[propertyPath] = false;
            
        // 获取枚举信息
        Type enumType = fieldInfo.FieldType;
        if (enumType.IsArray)
            enumType = enumType.GetElementType();
        else if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(List<>))
            enumType = enumType.GetGenericArguments()[0];
            
        var enumNames = Enum.GetNames(enumType);
        var enumValues = Enum.GetValues(enumType);
        
        // 过滤搜索结果
        var filteredItems = new List<(string name, int index)>();
        string searchText = _searchTexts[propertyPath].ToLower();
        
        for (int i = 0; i < enumNames.Length; i++)
        {
            if (string.IsNullOrEmpty(searchText) || enumNames[i].ToLower().Contains(searchText))
            {
                filteredItems.Add((enumNames[i], i));
            }
        }
        
        // 计算分页
        int totalPages = Mathf.CeilToInt((float)filteredItems.Count / attribute.ItemsPerPage);
        int currentPage = _currentPages[propertyPath];
        currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, totalPages - 1));
        _currentPages[propertyPath] = currentPage;
        
        // 绘制字段标签和主按钮
        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect buttonRect = EditorGUI.PrefixLabel(labelRect, label);
        
        string currentValueName = enumNames[property.enumValueIndex];
        string buttonText = attribute.ShowEnumValues ? 
            $"{currentValueName} ({property.enumValueIndex})" : 
            currentValueName;
            
        if (GUI.Button(buttonRect, buttonText, EditorStyles.popup))
        {
            _isDropdownOpen[propertyPath] = !_isDropdownOpen[propertyPath];
        }
        
        if (!_isDropdownOpen[propertyPath])
            return;
            
        // 绘制下拉内容时需要调整位置，使其对齐到按钮位置
        float yOffset = labelRect.yMax + 2;
        float dropdownX = buttonRect.x;
        float dropdownWidth = buttonRect.width;
        
        // 搜索框
        if (attribute.ShowSearchBar)
        {
            Rect searchRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginChangeCheck();
            _searchTexts[propertyPath] = EditorGUI.TextField(searchRect, "搜索:", _searchTexts[propertyPath]);
            if (EditorGUI.EndChangeCheck())
            {
                _currentPages[propertyPath] = 0; // 搜索时重置到第一页
            }
            yOffset += EditorGUIUtility.singleLineHeight + 2;
        }
        
        // 分页控件
        if (totalPages > 1)
        {
            Rect pageRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
            DrawPagingControls(pageRect, propertyPath, currentPage, totalPages);
            yOffset += EditorGUIUtility.singleLineHeight + 2;
        }
        
        // 绘制枚举项
        int startIndex = currentPage * attribute.ItemsPerPage;
        int endIndex = Mathf.Min(startIndex + attribute.ItemsPerPage, filteredItems.Count);
        
        for (int i = startIndex; i < endIndex; i++)
        {
            var item = filteredItems[i];
            Rect itemRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
            
            string itemText = attribute.ShowEnumValues ? 
                $"{item.name} ({item.index})" : 
                item.name;
                
            bool isSelected = property.enumValueIndex == item.index;
            
            if (isSelected)
            {
                EditorGUI.DrawRect(itemRect, new Color(0.3f, 0.5f, 0.8f, 0.3f));
            }
            
            if (GUI.Button(itemRect, itemText, EditorStyles.label))
            {
                property.enumValueIndex = item.index;
                _isDropdownOpen[propertyPath] = false;
                property.serializedObject.ApplyModifiedProperties();
            }
            
            yOffset += EditorGUIUtility.singleLineHeight;
        }
        
        // 点击外部关闭下拉框
        if (Event.current.type == EventType.MouseDown)
        {
            Rect totalRect = new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label));
            if (!totalRect.Contains(Event.current.mousePosition))
            {
                _isDropdownOpen[propertyPath] = false;
            }
        }
    }
    
    private void DrawPagingControls(Rect rect, string propertyPath, int currentPage, int totalPages)
    {
        float buttonWidth = 30f;
        float spacing = 5f;
        
        // 上一页按钮
        Rect prevRect = new Rect(rect.x, rect.y, buttonWidth, rect.height);
        if (GUI.Button(prevRect, "◄", EditorStyles.miniButtonLeft))
        {
            _currentPages[propertyPath] = Mathf.Max(0, currentPage - 1);
        }
        
        // 页面信息
        float infoWidth = rect.width - (buttonWidth * 2 + spacing * 2);
        Rect infoRect = new Rect(prevRect.xMax + spacing, rect.y, infoWidth, rect.height);
        EditorGUI.LabelField(infoRect, $"第 {currentPage + 1} 页 / 共 {totalPages} 页", EditorStyles.centeredGreyMiniLabel);
        
        // 下一页按钮
        Rect nextRect = new Rect(infoRect.xMax + spacing, rect.y, buttonWidth, rect.height);
        if (GUI.Button(nextRect, "►", EditorStyles.miniButtonRight))
        {
            _currentPages[propertyPath] = Mathf.Min(totalPages - 1, currentPage + 1);
        }
    }
}
