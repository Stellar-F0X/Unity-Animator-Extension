using System;
using AnimatorExpansion.Parameters;
using UnityEditor;
using UnityEngine;

namespace AnimatorExpansion.Editor
{
    internal class AnimationEventDrawer
    {
        public Action<SChangedValue, SChangedValue> onFocusedRangeSlider;
        public Action<int> onFocusedPointSlider;
        
        private const string _POINT_SLIDER_FOCUS_NAME_ = "__point_slider_field__";


        public void DrawDropdownSliderField(Rect position, SerializedProperty property, int index)
        {
            SerializedProperty sendType = property.FindPropertyRelative("sendType");
            Rect stateSendTypeRect = this.CalculateVariableRect(position, 0.15f, beforeEmpty: 5);
            EditorGUI.PropertyField(stateSendTypeRect, sendType, GUIContent.none);

            if ((EEventSendType)sendType.enumValueIndex == EEventSendType.Range)
            {
                SerializedProperty triggerTime = property.FindPropertyRelative("rangeTriggerTime");
                SerializedProperty min = triggerTime.FindPropertyRelative("min");
                SerializedProperty max = triggerTime.FindPropertyRelative("max");

                Rect minSliderFieldRect = CalculateConstantRect(position, 50, position.width * 0.15f, 10);
                Rect maxSliderFieldRect = CalculateConstantRect(position, 50, position.width - 50, 10);

                EditorGUI.FloatField(minSliderFieldRect, GUIContent.none, min.floatValue);
                EditorGUI.FloatField(maxSliderFieldRect, GUIContent.none, max.floatValue);

                float minFloatValue = min.floatValue;
                float maxFloatValue = max.floatValue;

                float sliderOffset = stateSendTypeRect.width + minSliderFieldRect.width;
                float sliderWidth = position.width - sliderOffset - 50;

                Rect rangeSliderFieldRect = CalculateConstantRect(position, sliderWidth, sliderOffset, 25);
                EditorGUI.MinMaxSlider(rangeSliderFieldRect, ref minFloatValue, ref maxFloatValue, 0f, 1f);

                bool minValueChanged = Mathf.Approximately(min.floatValue, minFloatValue) == false;
                bool maxValueChanged = Mathf.Approximately(max.floatValue, maxFloatValue) == false;
                
                min.floatValue = (float)Math.Round(minFloatValue, 3);
                max.floatValue = (float)Math.Round(maxFloatValue, 3);

                if (minValueChanged || maxValueChanged)
                {
                    onFocusedRangeSlider.Invoke(new SChangedValue(minValueChanged, minFloatValue), new SChangedValue(maxValueChanged, maxFloatValue));
                }
            }
            else if ((EEventSendType)sendType.enumValueIndex == EEventSendType.Point)
            {
                SerializedProperty triggerTime = property.FindPropertyRelative("triggerTime");
                Rect pointSliderFieldRect = CalculateVariableRect(position, 0.85f, position.width * 0.15f, 10);

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


        public void DrawStringHashField(Rect position, SerializedProperty property, EventReceiveInfo[] infos)
        {
            SerializedProperty eventName = property.FindPropertyRelative("eventName");
            SerializedProperty eventHash = property.FindPropertyRelative("eventHash");

            string[] popupNames = new string[infos.Length];
            
            for (int i = 0; i < infos.Length; ++i)
            {
                popupNames[i] = infos[i].eventName;
            }
            
            int selectedIndex = Array.IndexOf(popupNames, eventName.stringValue);
            
            Rect stateNameRect = this.CalculateVariableRect(position, 0.7f, 5, subtractWidth: 5);
            selectedIndex = EditorGUI.Popup(stateNameRect, selectedIndex, popupNames);
            eventName.stringValue = selectedIndex < 0 || selectedIndex >= infos.Length ?  "" : infos[selectedIndex].eventName;

            using (new EditorGUI.DisabledScope(true))
            {
                eventHash.intValue = Utility.StringToHash(eventName.stringValue);
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
    }
}