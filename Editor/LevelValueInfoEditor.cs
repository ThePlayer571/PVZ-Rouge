using UnityEngine;
using UnityEditor;
using TPL.PVZR.Classes.DataClasses.Level;

namespace TPL.PVZR.Editor
{
    [CustomPropertyDrawer(typeof(LevelValueDetail))]
    public class LevelValueDetailPropertyDrawer : PropertyDrawer
    {
        private bool showCalculationPanel = false;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            
            // 折叠面板标题
            showCalculationPanel = EditorGUI.Foldout(rect, showCalculationPanel, "MaxValueDetails", true);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
            if (showCalculationPanel)
            {
                EditorGUI.indentLevel++;
                
                // 获取序列化属性
                var recommendedDPSProp = property.FindPropertyRelative("RecommendedDPS");
                var validCellCountProp = property.FindPropertyRelative("ValidCellCount");
                var multiplierOfCellProp = property.FindPropertyRelative("MultiplierOfCell");
                var validSunpointProp = property.FindPropertyRelative("ValidSunpointWhenFinalWave");
                var multiplierOfSunpointProp = property.FindPropertyRelative("MultiplierOfSunpoint");
                
                // Cell 相关配置
                EditorGUI.LabelField(rect, "Cell Info", EditorStyles.boldLabel);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.PropertyField(rect, recommendedDPSProp);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.PropertyField(rect, validCellCountProp);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.PropertyField(rect, multiplierOfCellProp);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                // Sunpoint 相关配置
                rect.y += 5; // 添加间距
                EditorGUI.LabelField(rect, "Sunpoint Info", EditorStyles.boldLabel);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.PropertyField(rect, validSunpointProp);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.PropertyField(rect, multiplierOfSunpointProp);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                // 计算按钮
                rect.y += 10;
                rect.height = 30;
                var buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = 12;
                buttonStyle.fontStyle = FontStyle.Bold;
                
                if (GUI.Button(rect, "🔧 计算数据", buttonStyle))
                {
                    // 触发重绘以显示最新计算结果
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
                rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                rect.height = EditorGUIUtility.singleLineHeight;
                
                // 创建临时的 LevelValueDetail 对象来计算结果
                var tempDetail = new LevelValueDetail
                {
                    RecommendedDPS = recommendedDPSProp.floatValue,
                    ValidCellCount = validCellCountProp.intValue,
                    MultiplierOfCell = multiplierOfCellProp.floatValue,
                    ValidSunpointWhenFinalWave = validSunpointProp.intValue,
                    MultiplierOfSunpoint = multiplierOfSunpointProp.floatValue
                };
                
                // 显示计算结果区域
                rect.y += 5;
                EditorGUI.LabelField(rect, "计算结果", EditorStyles.boldLabel);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                // Cell 计算结果
                EditorGUI.BeginDisabledGroup(true);
                
                var maxValueCell = tempDetail.MaxValue_Cell;
                var recommendedValueCell = tempDetail.RecommendedValue_Cell;
                
                EditorGUI.FloatField(rect, "MaxValue_Cell", maxValueCell);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.FloatField(rect, "RecommendedValue_Cell", recommendedValueCell);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                // Sunpoint 计算结果
                var maxValueSunpoint = tempDetail.MaxValue_Sunpoint;
                var recommendedValueSunpoint = tempDetail.RecommendedValue_Sunpoint;
                
                EditorGUI.FloatField(rect, "MaxValue_Sunpoint", maxValueSunpoint);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.FloatField(rect, "RecommendedValue_Sunpoint", recommendedValueSunpoint);
                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                
                EditorGUI.EndDisabledGroup();
                
                // 显示计算公式说明
                var helpRect = rect;
                helpRect.height = 60;
                EditorGUI.HelpBox(helpRect, 
                    "计算公式:\n" +
                    "• DPS2Value(dps) = dps ÷ 13.3 × 10\n" +
                    "• Sunpoint2Value(sunpoint) = sunpoint ÷ 100 × 10\n" +
                    "• MaxValue_Cell = DPS2Value(RecommendedDPS) × ValidCellCount\n" +
                    "• RecommendedValue_Cell = MaxValue_Cell × MultiplierOfCell\n" +
                    "• MaxValue_Sunpoint = Sunpoint2Value(ValidSunpointWhenFinalWave)\n" +
                    "• RecommendedValue_Sunpoint = MaxValue_Sunpoint × MultiplierOfSunpoint", 
                    MessageType.Info);
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!showCalculationPanel)
                return EditorGUIUtility.singleLineHeight;
            
            // 计算展开时的总高度
            float height = EditorGUIUtility.singleLineHeight; // 折叠标题
            
            // Cell 配置区域
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Cell 标题
            height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3; // 3个Cell字段
            
            // Sunpoint 配置区域
            height += 5 + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Sunpoint 标题
            height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2; // 2个Sunpoint字段
            
            // 按钮和结果区域
            height += 10 + 30 + EditorGUIUtility.standardVerticalSpacing; // 计算按钮
            height += 5 + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // 结果标题
            height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 4; // 4个结果字段
            height += 60 + EditorGUIUtility.standardVerticalSpacing; // 帮助框
            
            return height;
        }
    }
}
