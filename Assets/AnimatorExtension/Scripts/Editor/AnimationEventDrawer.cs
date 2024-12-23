using System;
using AnimatorExtension.Parameters;
using UnityEditor;
using UnityEngine;

namespace AnimatorExtension.Editor
{
    internal class AnimationEventDrawer
    {
        private const string _POINT_SLIDER_FOCUS_NAME_ = "__point_slider_field__";

        public Action<int> onFocusedPointSlider;
        public Action<float> onFocusedRangeSlider;


        public void DrawDropdownSliderField(Rect position, SerializedProperty property, int index)
        {
            SerializedProperty sendType = property.FindPropertyRelative("sendType");
            Rect stateSendTypeRect = CustomEditorUtility.CalculateVariableRect(position, 0.15f, beforeEmpty: 5);
            EditorGUI.PropertyField(stateSendTypeRect, sendType, GUIContent.none);

            if ((EEventSendType)sendType.enumValueIndex == EEventSendType.Range)
            {
                SerializedProperty triggerTime = property.FindPropertyRelative("rangeTriggerTime");
                SerializedProperty min = triggerTime.FindPropertyRelative("min");
                SerializedProperty max = triggerTime.FindPropertyRelative("max");

                Rect minSliderFieldRect = CustomEditorUtility.CalculateConstantRect(position, 50, position.width * 0.15f, 10);
                Rect maxSliderFieldRect = CustomEditorUtility.CalculateConstantRect(position, 50, position.width - 50, 10);

                min.floatValue = EditorGUI.FloatField(minSliderFieldRect, GUIContent.none, min.floatValue);
                max.floatValue = EditorGUI.FloatField(maxSliderFieldRect, GUIContent.none, max.floatValue);

                if (min.floatValue > max.floatValue)
                {
                    max.floatValue = min.floatValue;
                }

                float minFloatValue = Mathf.Clamp01(min.floatValue);
                float maxFloatValue = Mathf.Clamp01(max.floatValue);

                float sliderOffset = stateSendTypeRect.width + minSliderFieldRect.width;
                float sliderWidth = position.width - sliderOffset - 50;

                Rect rangeSliderFieldRect = CustomEditorUtility.CalculateConstantRect(position, sliderWidth, sliderOffset, 25);
                EditorGUI.MinMaxSlider(rangeSliderFieldRect, ref minFloatValue, ref maxFloatValue, 0f, 1f);

                bool minValueChanged = Mathf.Approximately(min.floatValue, minFloatValue) == false;
                bool maxValueChanged = Mathf.Approximately(max.floatValue, maxFloatValue) == false;

                min.floatValue = (float)Math.Round(minFloatValue, 3);
                max.floatValue = (float)Math.Round(maxFloatValue, 3);

                if (minValueChanged || maxValueChanged)
                {
                    GUI.FocusControl(string.Empty);

                    if (minValueChanged && maxValueChanged)
                    {
                        onFocusedRangeSlider.Invoke((minFloatValue + maxFloatValue) * 0.5f);
                    }
                    else if (minValueChanged)
                    {
                        onFocusedRangeSlider.Invoke(minFloatValue);
                    }
                    else if (maxValueChanged)
                    {
                        onFocusedRangeSlider.Invoke(maxFloatValue);
                    }
                }
            }
            else if ((EEventSendType)sendType.enumValueIndex == EEventSendType.Point)
            {
                SerializedProperty triggerTime = property.FindPropertyRelative("triggerTime");
                Rect pointSliderFieldRect = CustomEditorUtility.CalculateVariableRect(position, 0.85f, position.width * 0.15f, 10);

                GUI.SetNextControlName(_POINT_SLIDER_FOCUS_NAME_);
                EditorGUI.Slider(pointSliderFieldRect, triggerTime, 0f, 1f, GUIContent.none);
                string focusPropertyName = GUI.GetNameOfFocusedControl();

                if (string.IsNullOrEmpty(focusPropertyName) || string.IsNullOrWhiteSpace(focusPropertyName))
                {
                    return;
                }

                if (string.Compare(focusPropertyName, _POINT_SLIDER_FOCUS_NAME_, StringComparison.Ordinal) == 0)
                {
                    onFocusedPointSlider?.Invoke(index);
                }
            }
        }


        public int DrawStringHashField(Rect position, SerializedProperty property, string[] eventNames)
        {
            SerializedProperty eventName = property.FindPropertyRelative("eventName");
            SerializedProperty eventHash = property.FindPropertyRelative("eventHash");

            int selectedIndex = Array.IndexOf(eventNames, eventName.stringValue);

            Rect stateNameRect = CustomEditorUtility.CalculateVariableRect(position, 0.7f, 5, subtractWidth: 5);
            selectedIndex = EditorGUI.Popup(stateNameRect, selectedIndex, eventNames);
            eventName.stringValue = selectedIndex < 0 || selectedIndex >= eventNames.Length ? eventNames[0] : eventNames[selectedIndex];

            using (new EditorGUI.DisabledScope(true))
            {
                eventHash.intValue = Extension.StringToHash(eventName.stringValue);
                Rect stateHashRect = CustomEditorUtility.CalculateVariableRect(position, 0.3f, position.width * 0.7f, 5);
                EditorGUI.PropertyField(stateHashRect, eventHash, GUIContent.none);
            }

            return selectedIndex;
        }



        public void DrawParameterField(Rect position, SerializedProperty property, EParameterType parameterTypes)
        {
            SerializedProperty parameter = property.FindPropertyRelative("parameter");
            SerializedProperty parameterType = parameter.FindPropertyRelative("parameterType");

            Rect paramTypeRect = CustomEditorUtility.CalculateVariableRect(position, 0.15f, beforeEmpty: 5);
            Rect parameterRect = CustomEditorUtility.CalculateVariableRect(position, 0.85f, position.width * 0.15f, 10);

            parameterType.enumValueIndex = (int)parameterTypes;

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.PropertyField(paramTypeRect, parameterType, GUIContent.none);
            }

            switch (parameterTypes)
            {
                case EParameterType.Int:
                    SerializedProperty intProp = parameter.FindPropertyRelative("intValue");
                    intProp.intValue = EditorGUI.IntField(parameterRect, intProp.intValue);
                    break;

                case EParameterType.Float:
                    SerializedProperty floatProp = parameter.FindPropertyRelative("floatValue");
                    floatProp.floatValue = EditorGUI.FloatField(parameterRect, floatProp.floatValue);
                    break;

                case EParameterType.Bool:
                    SerializedProperty boolProp = parameter.FindPropertyRelative("boolValue");
                    boolProp.boolValue = EditorGUI.Toggle(parameterRect, boolProp.boolValue);
                    break;

                case EParameterType.String:
                    SerializedProperty stringProp = parameter.FindPropertyRelative("stringValue");
                    stringProp.stringValue = EditorGUI.TextField(parameterRect, stringProp.stringValue);
                    break;

                case EParameterType.Vector3:
                    SerializedProperty vector3Prop = parameter.FindPropertyRelative("vector3Value");
                    vector3Prop.vector3Value = EditorGUI.Vector3Field(parameterRect, GUIContent.none, vector3Prop.vector3Value);
                    break;

                case EParameterType.Quaternion:
                    SerializedProperty quaternionProp = parameter.FindPropertyRelative("quaternionValue");
                    Quaternion quaternion = quaternionProp.quaternionValue;
                    Vector4 vector4 = new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
                    vector4 = EditorGUI.Vector4Field(parameterRect, GUIContent.none, vector4);
                    quaternionProp.quaternionValue = new Quaternion(vector4.x, vector4.y, vector4.z, vector4.w);
                    break;

                case EParameterType.GameObject:
                    SerializedProperty gameObjectProp = parameter.FindPropertyRelative("gameObjectValue");
                    gameObjectProp.objectReferenceValue =
                        EditorGUI.ObjectField(parameterRect, GUIContent.none, gameObjectProp.objectReferenceValue, typeof(GameObject), true);

                    break;

                case EParameterType.Color:
                    SerializedProperty colorProp = parameter.FindPropertyRelative("colorValue");
                    colorProp.colorValue = EditorGUI.ColorField(parameterRect, GUIContent.none, colorProp.colorValue);
                    break;
            }
        }
    }
}