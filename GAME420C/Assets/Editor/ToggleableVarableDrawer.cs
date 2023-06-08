using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ToggleableVarableAttribute))]
public class ToggleableVarableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ToggleableVarableAttribute toggleable = (ToggleableVarableAttribute)attribute;
        bool enabled = GetToggleableHideAttributeResult(toggleable, property);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;

        if (!toggleable.HideIfTrue || enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ToggleableVarableAttribute toggleable = (ToggleableVarableAttribute)attribute;
        bool enabled = GetToggleableHideAttributeResult(toggleable, property);

        if (!toggleable.HideIfTrue || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    private bool GetToggleableHideAttributeResult(ToggleableVarableAttribute toggleableVarable, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath;
        string toggleablePath = propertyPath.Replace(property.name, toggleableVarable.ConditionalVariablefield);
        SerializedProperty sourceProperty = property.serializedObject.FindProperty(toggleablePath);

        if (sourceProperty != null)
        {
            enabled = sourceProperty.boolValue;
        }
        else
        {
            Debug.LogWarning("Attempting to use a Toggleable Variable but no matching SourcePropertyValue found in object: " + toggleableVarable.ConditionalVariablefield);
        }

        return enabled;
    }
}
