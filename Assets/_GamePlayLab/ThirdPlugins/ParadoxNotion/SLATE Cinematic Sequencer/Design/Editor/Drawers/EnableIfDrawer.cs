#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Slate
{

    [CustomPropertyDrawer(typeof(EnabledIfAttribute))]
    public class EnableIfDrawer : PropertyDrawer
    {
        private bool enable;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var att = (EnabledIfAttribute)attribute;
            var parent = ReflectionTools.GetRelativeMemberParent(property.serializedObject.targetObject, property.propertyPath);
            var member = ReflectionTools.RTGetFieldOrProp(parent.GetType(), att.propertyName);
            var value = (int)System.Convert.ChangeType(member.RTGetFieldOrPropValue(parent), typeof(int));
            enable = value == att.value;
            GUI.enabled = enable;
            EditorGUI.PropertyField(position, property);
            GUI.enabled = true;
        }
    }
}

#endif