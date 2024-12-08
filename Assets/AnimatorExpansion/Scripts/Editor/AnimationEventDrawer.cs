using System;
using AnimatorExpansion.Parameters;
using UnityEditor;
using UnityEngine;

namespace AnimatorExpansion.Editor
{
    public class AnimationEventDrawer
    {
        public void DrawDropdownSliderField(Rect position, SerializedProperty property)
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
                Rect maxSliderFieldRect = CalculateConstantRect(position, 50, position.width - 50, 10,0);
                
                EditorGUI.FloatField(minSliderFieldRect, GUIContent.none, min.floatValue);
                EditorGUI.FloatField(maxSliderFieldRect, GUIContent.none, max.floatValue);
            
                float minFloatValue = min.floatValue;
                float maxFloatValue = max.floatValue;

                float sliderOffset = stateSendTypeRect.width + minSliderFieldRect.width;
                float sliderWidth = position.width - sliderOffset - 50;
                
                Rect sliderFieldRect = CalculateConstantRect(position, sliderWidth, sliderOffset, 25);
                EditorGUI.MinMaxSlider(sliderFieldRect, ref minFloatValue, ref maxFloatValue, 0f, 1f);
                
                min.floatValue = (float)Math.Round(minFloatValue, 3);
                max.floatValue = (float)Math.Round(maxFloatValue, 3);
            }
            else if ((EEventSendType)sendType.enumValueIndex == EEventSendType.Specific)
            {
                SerializedProperty triggerTime = property.FindPropertyRelative("triggerTime");
                Rect stateSliderRect = CalculateVariableRect(position, 0.85f, position.width * 0.15f, 10);
                EditorGUI.Slider(stateSliderRect, triggerTime, 0f, 1f, GUIContent.none);
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

        
        public Rect CalculateVariableRect(Rect position, float widthPercentage, float horizontalOffset = 0, float beforeEmpty = 0, float subtractWidth = 0)
        {
            float width = Mathf.Clamp(position.width * widthPercentage - beforeEmpty - subtractWidth, 0, position.width);

            return new Rect(position.x + beforeEmpty + horizontalOffset, position.y, width, EditorGUIUtility.singleLineHeight);
        }
        
        
        public Rect CalculateConstantRect(Rect position, float fieldWidth, float horizontalOffset = 0, float beforeEmpty = 0, float subtractWidth = 0)
        {
            float width = Mathf.Clamp(fieldWidth - beforeEmpty - subtractWidth, 0, position.width);

            return new Rect(position.x + beforeEmpty + horizontalOffset, position.y, width, EditorGUIUtility.singleLineHeight);
        }
    }
}