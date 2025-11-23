using Gas.Variable;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FloatVariable))]
public class FloatVariableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var valueProp = property.FindPropertyRelative("_value");
        var defaultProp = property.FindPropertyRelative("_defaultValue");

        // Draw all child properties (value, tag, event, etc.)
        EditorGUI.PropertyField(position, property, label, true);

        // After drawing, sync default
        if (valueProp.floatValue != defaultProp.floatValue)
            defaultProp.floatValue = valueProp.floatValue;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

}