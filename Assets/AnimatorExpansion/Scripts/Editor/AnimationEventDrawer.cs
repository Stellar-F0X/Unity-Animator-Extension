using System;
using AnimatorExpansion.Parameters;
using UnityEditor;
using UnityEngine;

namespace AnimatorExpansion.Editor
{
    public class AnimationEventDrawer
    {
        public Action<int> onFocusedRangeSlider;
        public Action<int> onFocusedPointSlider;

        private const string _RANGE_SLIDER_FOCUS_NAME_ = "__range_slider_field__";
        private const string _POINT_SLIDER_FOCUS_NAME_ = "__point_slider_field__";


        public void DrawDropdownSliderField(Rect position, SerializedProperty property, int index)
        {
            SerializedProperty sendType = property.FindPropertyRelative("sendType");
            Rect stateSendTypeRect = this.CalculateVariableRect(position, 0.15f, beforeEmpty: 5);
            EditorGUI.PropertyField(stateSendTypeRect, sendType, GUIContent.none);

            if ((EEventSendType)sendType.enumValueIndex == EEventSendType.Range)
            {
                SerializedProperty triggerTime = property.FindPropertyRelative("repeatTriggerRange");
                SerializedProperty min = triggerTime.FindPropertyRelative("min");
                SerializedProperty max = triggerTime.FindPropertyRelative("max");

                Rect minSliderFieldRect = CalculateConstantRect(position, 50, position.width * 0.15f, 10, 0);
                Rect maxSliderFieldRect = CalculateConstantRect(position, 50, position.width - 50, 10, 0);

                EditorGUI.FloatField(minSliderFieldRect, GUIContent.none, min.floatValue);
                EditorGUI.FloatField(maxSliderFieldRect, GUIContent.none, max.floatValue);

                float minFloatValue = min.floatValue;
                float maxFloatValue = max.floatValue;

                float sliderOffset = stateSendTypeRect.width + minSliderFieldRect.width;
                float sliderWidth = position.width - sliderOffset - 50;

                Rect rangeSliderFieldRect = CalculateConstantRect(position, sliderWidth, sliderOffset, 25);

                GUI.SetNextControlName(_RANGE_SLIDER_FOCUS_NAME_);
                EditorGUI.MinMaxSlider(rangeSliderFieldRect, ref minFloatValue, ref maxFloatValue, 0f, 1f);
                this.CheckFocusFieldAndCallAction(GUI.GetNameOfFocusedControl(), _RANGE_SLIDER_FOCUS_NAME_, index, onFocusedRangeSlider);

                min.floatValue = (float)Math.Round(minFloatValue, 3);
                max.floatValue = (float)Math.Round(maxFloatValue, 3);
            }
            else if ((EEventSendType)sendType.enumValueIndex == EEventSendType.Point)
            {
                SerializedProperty triggerTime = property.FindPropertyRelative("triggerTime");
                Rect pointSliderFieldRect = CalculateVariableRect(position, 0.85f, position.width * 0.15f, 10);

                GUI.SetNextControlName(_POINT_SLIDER_FOCUS_NAME_);
                EditorGUI.Slider(pointSliderFieldRect, triggerTime, 0f, 1f, GUIContent.none);
                this.CheckFocusFieldAndCallAction(GUI.GetNameOfFocusedControl(), _POINT_SLIDER_FOCUS_NAME_, index, onFocusedPointSlider);
            }
        }


        public void DrawStringHashField(Rect position, SerializedProperty property)
        {
            SerializedProperty eventName = property.FindPropertyRelative("eventName");
            SerializedProperty eventHash = property.FindPropertyRelative("eventHash");

            Rect stateNameRect = this.CalculateVariableRect(position, 0.7f, 5, subtractWidth: 5);
            eventName.stringValue = EditorGUI.TextField(stateNameRect, eventName.stringValue);

            using (new EditorGUI.DisabledScope(true))
            {
                eventHash.intValue = string.IsNullOrWhiteSpace(eventName.stringValue) ? 0 : Utility.StringToHash(eventName.stringValue);
                Rect stateHashRect = this.CalculateVariableRect(position, 0.3f, position.width * 0.7f, 5);
                EditorGUI.PropertyField(stateHashRect, eventHash, GUIContent.none);
            }
        }


        private Rect CalculateVariableRect(Rect position, float widthPercentage, float horizontalOffset = 0, float beforeEmpty = 0, float subtractWidth = 0)
        {
            float width = Mathf.Clamp(position.width * widthPercentage - beforeEmpty - subtractWidth, 0, position.width);

            return new Rect(position.x + beforeEmpty + horizontalOffset, position.y, width, EditorGUIUtility.singleLineHeight);
        }


        private Rect CalculateConstantRect(Rect position, float fieldWidth, float horizontalOffset = 0, float beforeEmpty = 0, float subtractWidth = 0)
        {
            float width = Mathf.Clamp(fieldWidth - beforeEmpty - subtractWidth, 0, position.width);

            return new Rect(position.x + beforeEmpty + horizontalOffset, position.y, width, EditorGUIUtility.singleLineHeight);
        }


        private void CheckFocusFieldAndCallAction(in string focusPropertyName, in string targetName, int index, Action<int> aciton)
        {
            if (string.IsNullOrEmpty(focusPropertyName) || string.IsNullOrWhiteSpace(focusPropertyName))
            {
                return;
            }

            if (string.Compare(focusPropertyName, targetName, StringComparison.Ordinal) == 0)
            {
                aciton?.Invoke(index);
            }
        }
    }
}