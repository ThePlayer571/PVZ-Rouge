using UnityEditor;
using UnityEngine;
using TPL.PVZR.Tools;

[CustomEditor(typeof(TriggerDetector))]
public class TriggerDetectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 获取目标对象
        TriggerDetector triggerDetector = (TriggerDetector)target;

        // 显示 targetCount
        EditorGUILayout.LabelField("Target Count", triggerDetector.HasTarget ? triggerDetector.DetectedTargets.Count.ToString() : "0");

        // 显示 RecordTargets 状态
        EditorGUILayout.LabelField("Record Targets", triggerDetector.RecordTargets ? "Enabled" : "Disabled");

        // 显示 _detectedTargets 中的所有目标名称
        EditorGUILayout.LabelField("Detected Targets:");
        if (triggerDetector.DetectedTargets.Count > 0)
        {
            foreach (var target in triggerDetector.DetectedTargets)
            {
                EditorGUILayout.LabelField("- " + target.gameObject.name);
            }
        }
        else
        {
            EditorGUILayout.LabelField("None");
        }

        // 显示是否存在 TargetPredicate
        EditorGUILayout.LabelField("Has Target Predicate", triggerDetector.TargetPredicate != null ? "Yes" : "No");

        // 显示并允许调试 DEBUG_AlwaysReturnHasTarget
        triggerDetector.DebugAlwaysReturnHasTarget = EditorGUILayout.Toggle("Debug Always Return Has Target", triggerDetector.DebugAlwaysReturnHasTarget);

        // 保存更改
        if (GUI.changed)
        {
            EditorUtility.SetDirty(triggerDetector);
        }
    }
}
