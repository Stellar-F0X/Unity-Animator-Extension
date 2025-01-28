using System;
using AnimatorExtension.Parameters;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AnimatorExtension.Editor
{
    internal class AnimationEventDrawer
    {
        private const string _POINT_SLIDER_FOCUS_NAME_ = "__point_slider_field__";

        public Action<int> onFocusedPointSlider;
        public Action<float> onFocusedRangeSlider;


        public void DrawDropdownSliderField(Rect position, SerializedProperty property, int index)
        {
            SerializedProperty dispatchType = property.FindPropertyRelative("dispatchType");
            Rect stateSendTypeRect = CustomEditorUtility.CalculateVariableRect(position, 0.15f, beforeEmpty: 5);
            EditorGUI.PropertyField(stateSendTypeRect, dispatchType, GUIContent.none);

            if ((EEventDispatchType)dispatchType.enumValueIndex == EEventDispatchType.Range)
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
            else if ((EEventDispatchType)dispatchType.enumValueIndex == EEventDispatchType.Point)
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

                if (focusPropertyName.Compare(_POINT_SLIDER_FOCUS_NAME_))
                {
                    onFocusedPointSlider?.Invoke(index);
                }
            }
        }


        public int DrawStringHashField(Rect position, SerializedProperty property, string[] eventNames, int[] hashes)
        {
            SerializedProperty eventName = property.FindPropertyRelative("eventName");
            SerializedProperty eventHash = property.FindPropertyRelative("eventHash");

            int selectedIndex = Array.IndexOf(eventNames, eventName.stringValue);

            Rect stateNameRect = CustomEditorUtility.CalculateVariableRect(position, 0.7f, 5, subtractWidth: 5);
            selectedIndex = EditorGUI.Popup(stateNameRect, selectedIndex, eventNames);
            eventName.stringValue = selectedIndex < 0 || selectedIndex >= eventNames.Length ? eventNames[0] : eventNames[selectedIndex];

            if (selectedIndex != 0)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    eventHash.intValue = hashes[selectedIndex];
                    Rect stateHashRect = CustomEditorUtility.CalculateVariableRect(position, 0.3f, position.width * 0.7f, 5);
                    EditorGUI.PropertyField(stateHashRect, eventHash, GUIContent.none);
                }
            }
            
            return selectedIndex;
        }



        public void DrawParameterField(Rect position, SerializedProperty property, EAnimationEventParameter animationEventParameter)
        {
            SerializedProperty parameter = property.FindPropertyRelative("parameter");
            SerializedProperty parameterType = parameter.FindPropertyRelative("parameterType");

            Rect paramTypeRect = CustomEditorUtility.CalculateVariableRect(position, 0.15f, beforeEmpty: 5);
            Rect parameterRect = CustomEditorUtility.CalculateVariableRect(position, 0.85f, position.width * 0.15f, 10);

            parameterType.enumValueIndex = (int)animationEventParameter;

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.PropertyField(paramTypeRect, parameterType, GUIContent.none);
            }

            switch (animationEventParameter)
            {
                case EAnimationEventParameter.Int:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("intValue");
                    prop.intValue = EditorGUI.IntField(parameterRect, prop.intValue);
                    return;
                }
                case EAnimationEventParameter.Float:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("floatValue");
                    prop.floatValue = EditorGUI.FloatField(parameterRect, prop.floatValue);
                    return;
                }
                case EAnimationEventParameter.Bool:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("boolValue");
                    prop.boolValue = EditorGUI.Toggle(parameterRect, prop.boolValue);
                    return;
                }
                case EAnimationEventParameter.String:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("stringValue");
                    prop.stringValue = EditorGUI.TextField(parameterRect, prop.stringValue);
                    return;
                }
                case EAnimationEventParameter.Vector2:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("vector2Value");
                    prop.vector2Value = EditorGUI.Vector2Field(parameterRect, GUIContent.none, prop.vector2Value);
                    return;
                }
                case EAnimationEventParameter.Vector3:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("vector3Value");
                    prop.vector3Value = EditorGUI.Vector3Field(parameterRect, GUIContent.none, prop.vector3Value);
                    return;
                }
                case EAnimationEventParameter.Quaternion:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("quaternionValue");
                    Quaternion quaternion = prop.quaternionValue;
                    Vector4 vector4 = new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
                    vector4 = EditorGUI.Vector4Field(parameterRect, GUIContent.none, vector4);
                    prop.quaternionValue = new Quaternion(vector4.x, vector4.y, vector4.z, vector4.w);
                    return;
                }
                case EAnimationEventParameter.Color:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("colorValue");
                    prop.colorValue = EditorGUI.ColorField(parameterRect, GUIContent.none, prop.colorValue);
                    return;
                }
                case EAnimationEventParameter.GameObject:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("gobjValue");
                    Object obj = prop.objectReferenceValue;
                    obj = EditorGUI.ObjectField(parameterRect, obj, typeof(GameObject), true);
                    prop.objectReferenceValue = obj;
                    return;
                }
                case EAnimationEventParameter.ScriptableObject:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("sobjValue");
                    Object obj = prop.objectReferenceValue;
                    obj = EditorGUI.ObjectField(parameterRect, obj, typeof(ScriptableObject), false);
                    prop.objectReferenceValue = obj;
                    return;
                }
                case EAnimationEventParameter.Tag:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("stringValue");
                    string[] tagList = InternalEditorUtility.tags;
                    int selectedIndex = Array.IndexOf(tagList, prop.stringValue);
                    selectedIndex = Mathf.Clamp(selectedIndex, 0, tagList.Length - 1);
                    prop.stringValue = tagList[EditorGUI.Popup(parameterRect, selectedIndex, tagList)];
                    return;
                }
                case EAnimationEventParameter.LayerMask:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("intValue");
                    int layerIndex = (int)Mathf.Log(prop.intValue, 2);
                    string[] layerList = InternalEditorUtility.layers;
                    prop.intValue = 1 << EditorGUI.Popup(parameterRect, layerIndex, layerList);
                    return;
                }
                case EAnimationEventParameter.AnimationCurve:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("curveValue");
                    AnimationCurve animationCurve = prop.animationCurveValue;
                    prop.animationCurveValue = EditorGUI.CurveField(parameterRect, animationCurve);
                    return;
                }
                case EAnimationEventParameter.Customization:
                {
                    SerializedProperty prop = parameter.FindPropertyRelative("customValue");
                    parameterRect.x += 10;
                    parameterRect.width -= 10;
                    EditorGUI.PropertyField(parameterRect, prop, new GUIContent(prop.displayName), true);
                    return;
                }
            }
        }
    }
}