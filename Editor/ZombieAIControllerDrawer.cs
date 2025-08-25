using UnityEditor;
using UnityEngine;
using TPL.PVZR.Classes.ZombieAI.Public;
using TPL.PVZR.ViewControllers.Entities.Zombies;
using TPL.PVZR.ViewControllers.Entities.Zombies.Base;

[CustomPropertyDrawer(typeof(ZombieAIController))]
public class ZombieAIControllerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        // 直接通过反射获取currentMoveData
        object zombieAIControllerObj = property.serializedObject.targetObject;
        var zombieAIControllerField = zombieAIControllerObj.GetType().GetField(property.name);
        ZombieAIController controller = null;
        if (zombieAIControllerField != null)
        {
            controller = zombieAIControllerField.GetValue(zombieAIControllerObj) as ZombieAIController;
        }
        else if (zombieAIControllerObj is Zombie zombie)
        {
            controller = zombie.zombieAIController;
        }

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float y = position.y;
        Rect rect;

        // MoveData标题
        rect = new Rect(position.x, y, position.width, lineHeight);
        EditorGUI.LabelField(rect, "MoveData", EditorStyles.boldLabel);
        y += lineHeight + spacing;

        if (controller != null && controller.currentMoveData != null)
        {
            MoveData moveData = controller.currentMoveData;
            rect = new Rect(position.x, y, position.width, lineHeight);
            EditorGUI.LabelField(rect, "moveType", moveData.moveType.ToString());
            y += lineHeight + spacing;
            rect = new Rect(position.x, y, position.width, lineHeight);
            EditorGUI.LabelField(rect, "target", moveData.target.ToString());
            y += lineHeight + spacing;
            rect = new Rect(position.x, y, position.width, lineHeight);
            EditorGUI.LabelField(rect, "from", moveData.from.ToString());
            y += lineHeight + spacing;
            rect = new Rect(position.x, y, position.width, lineHeight);
            EditorGUI.LabelField(rect, "moveStage", moveData.moveStage.ToString());
        }
        else
        {
            rect = new Rect(position.x, y, position.width, lineHeight);
            EditorGUI.LabelField(rect, "MoveData is null", EditorStyles.centeredGreyMiniLabel);
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 1行标题 + 4行数据 + 间距
        return EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 4;
    }
}
